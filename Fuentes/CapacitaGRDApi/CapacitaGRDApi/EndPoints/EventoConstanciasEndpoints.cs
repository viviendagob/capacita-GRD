using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.Security.Claims;

namespace CapacitaGRDApi.EndPoints
{
    public static class EventoConstanciasEndpoints
    {
        public static RouteGroupBuilder MapEventoConstancias(this RouteGroupBuilder group)
        {
            group.MapGet("/elegibles/{idEvento:int}", Elegibles).RequireAuthorization();
            group.MapPost("/generar", Generar).RequireAuthorization();
            group.MapGet("/buscar", Buscar).RequireAuthorization();
            group.MapGet("/{idConstancia:int}/descargar", Descargar).RequireAuthorization();
            group.MapPut("/{idConstancia:int}/anular", Anular).RequireAuthorization();
            group.MapGet("/verificar/{codigo}", Verificar).AllowAnonymous();

            return group;
        }

        static async Task<Ok<List<ParticipanteConstanciaDTO>>> Elegibles(
            int idEvento
            , IRepositorioEventoConstancias repositorio)
        {
            var lista = await repositorio.ListarElegibles(idEvento);
            return TypedResults.Ok(lista);
        }

        static async Task<Results<Ok<ConstanciaDTO>, BadRequest<string>, NotFound>> Generar(
            int idEvento
            , int idPersona
            , IRepositorioEventoConstancias repositorioConstancias
            , IRepositorioEventos repositorioEventos
            , IRepositorioPersonas repositorioPersonas
            , IWebHostEnvironment env
            , IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor
            , ClaimsPrincipal usuario)
        {
            var elegibles = await repositorioConstancias.ListarElegibles(idEvento);
            var elegible = elegibles.FirstOrDefault(e => e.ID_PERSONA == idPersona);
            if (elegible is null)
            {
                return TypedResults.NotFound();
            }
            if (!elegible.APTO)
            {
                return TypedResults.BadRequest(
                    "El participante no cumple los requisitos (asistencia a sesiones obligatorias y/o encuesta de satisfacción).");
            }

            // Ya generada y vigente: se devuelve la existente en vez de duplicar.
            var existente = await repositorioConstancias.ObtenerPorEventoPersona(idEvento, idPersona);
            if (existente is not null && existente.ESTADO == "GENERADA")
            {
                return TypedResults.Ok(MapDTO(existente));
            }

            var resultEvento = await repositorioEventos.Obtener(idEvento);
            var evento = resultEvento.FirstOrDefault();
            if (evento is null) return TypedResults.NotFound();

            var resultPersona = await repositorioPersonas.Obtener(idPersona);
            var persona = resultPersona.FirstOrDefault();
            if (persona is null) return TypedResults.NotFound();

            var codigo = "CGRD-" + DateTime.UtcNow.ToString("yyyy") + "-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
            var baseUrl = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var urlVerificacion = $"{baseUrl}/constancias/verificar/{codigo}";

            var carpetaEventos = configuration.GetSection("folferEventos").Value;
            var folderConstancias = Path.Combine(env.WebRootPath, carpetaEventos, evento.COD_EVENTO, "constancias");
            if (!Directory.Exists(folderConstancias))
            {
                Directory.CreateDirectory(folderConstancias);
            }

            var qrBytes = GenerarQRBytes(urlVerificacion);
            var nombreArchivo = codigo + ".pdf";
            var rutaPdf = Path.Combine(folderConstancias, nombreArchivo);

            var nombreCompleto = $"{persona.NOMBRES} {persona.APELLIDO_PATERNO} {persona.APELLIDO_MATERNO}".Trim();
            GenerarConstanciaPdf(rutaPdf, nombreCompleto, evento.NOMBRE, evento.FECHA_INICIO, evento.FECHA_FIN, codigo, qrBytes);

            var urlPdf = Path.Combine(baseUrl, carpetaEventos, evento.COD_EVENTO, "constancias", nombreArchivo).Replace("\\", "/");

            var constancia = new EventoConstancia
            {
                ID_EVENTO = idEvento,
                ID_PERSONA = idPersona,
                CODIGO = codigo,
                URL_PDF = urlPdf,
                USER_GENERACION = usuario.Identity?.Name ?? "sistema",
            };
            var id = await repositorioConstancias.Agregar(constancia);
            var creada = await repositorioConstancias.Obtener(id);

            return TypedResults.Ok(MapDTO(creada!));
        }

        static async Task<Ok<List<ConstanciaDTO>>> Buscar(
            string? documento
            , int? idEvento
            , string? codigo
            , IRepositorioEventoConstancias repositorio)
        {
            var lista = await repositorio.Buscar(documento, idEvento, codigo);
            return TypedResults.Ok(lista);
        }

        static async Task<Results<Ok<ConstanciaDTO>, NotFound>> Descargar(
            int idConstancia
            , IRepositorioEventoConstancias repositorio)
        {
            var constancia = await repositorio.Obtener(idConstancia);
            if (constancia is null) return TypedResults.NotFound();

            await repositorio.RegistrarDescarga(idConstancia);
            constancia.NUM_DESCARGAS += 1;
            return TypedResults.Ok(MapDTO(constancia));
        }

        static async Task<Results<NoContent, NotFound>> Anular(
            int idConstancia
            , AnularConstanciaDTO body
            , IRepositorioEventoConstancias repositorio
            , ClaimsPrincipal usuario)
        {
            var constancia = await repositorio.Obtener(idConstancia);
            if (constancia is null) return TypedResults.NotFound();

            await repositorio.Anular(idConstancia, body.MOTIVO, usuario.Identity?.Name ?? "sistema");
            return TypedResults.NoContent();
        }

        static async Task<Ok<VerificarConstanciaDTO>> Verificar(
            string codigo
            , IRepositorioEventoConstancias repositorioConstancias
            , IRepositorioEventos repositorioEventos
            , IRepositorioPersonas repositorioPersonas)
        {
            var constancia = await repositorioConstancias.ObtenerPorCodigo(codigo);
            if (constancia is null)
            {
                return TypedResults.Ok(new VerificarConstanciaDTO { VALIDA = false, MENSAJE = "Código no encontrado." });
            }

            var resultEvento = await repositorioEventos.Obtener(constancia.ID_EVENTO);
            var evento = resultEvento.FirstOrDefault();
            var resultPersona = await repositorioPersonas.Obtener(constancia.ID_PERSONA);
            var persona = resultPersona.FirstOrDefault();

            if (constancia.ESTADO == "ANULADA")
            {
                return TypedResults.Ok(new VerificarConstanciaDTO
                {
                    VALIDA = false,
                    CODIGO = constancia.CODIGO,
                    ESTADO = constancia.ESTADO,
                    MENSAJE = "Esta constancia fue anulada y ya no es válida.",
                });
            }

            return TypedResults.Ok(new VerificarConstanciaDTO
            {
                VALIDA = true,
                CODIGO = constancia.CODIGO,
                ESTADO = constancia.ESTADO,
                NOMBRE_COMPLETO = persona is null ? null : $"{persona.NOMBRES} {persona.APELLIDO_PATERNO} {persona.APELLIDO_MATERNO}".Trim(),
                NOMBRE_EVENTO = evento?.NOMBRE,
                FECHA_INICIO = evento?.FECHA_INICIO,
                FECHA_FIN = evento?.FECHA_FIN,
                MENSAJE = "Constancia válida.",
            });
        }

        static byte[] GenerarQRBytes(string contenido)
        {
            var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            var qrCode = qrEncoder.Encode(contenido);
            var renderer = new GraphicsRenderer(new FixedModuleSize(6, QuietZoneModules.Two), Brushes.Black, Brushes.White);
            using var stream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, imageFormat: System.Drawing.Imaging.ImageFormat.Png, stream);
            return stream.ToArray();
        }

        // Diseño propio (no había plantilla previa que replicar): A4 horizontal, encabezado
        // institucional, nombre del participante destacado, datos del evento, QR de
        // verificación + código único abajo, y líneas de firma.
        static void GenerarConstanciaPdf(
            string rutaPdf
            , string nombreParticipante
            , string nombreEvento
            , DateTime fechaInicio
            , DateTime fechaFin
            , string codigo
            , byte[] qrBytes)
        {
            var periodo = fechaInicio.Date == fechaFin.Date
                ? fechaInicio.ToString("dd 'de' MMMM 'de' yyyy")
                : $"del {fechaInicio:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}";

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(f => f.FontFamily("Arial"));

                    page.Content().Column(col =>
                    {
                        col.Item().AlignCenter().PaddingTop(10).Text("MINISTERIO DE VIVIENDA, CONSTRUCCIÓN Y SANEAMIENTO")
                            .FontSize(11).SemiBold().FontColor("#003865");
                        col.Item().AlignCenter().Text("Unidad de Gestión del Riesgo de Desastres - UGERDES")
                            .FontSize(9).FontColor("#666666");

                        col.Item().PaddingTop(30).AlignCenter().Text("CONSTANCIA DE PARTICIPACIÓN")
                            .FontSize(26).Bold().FontColor("#003865");

                        col.Item().PaddingTop(30).AlignCenter().Text("Se otorga la presente constancia a:")
                            .FontSize(13);

                        col.Item().PaddingTop(10).AlignCenter().Text(nombreParticipante.ToUpper())
                            .FontSize(22).Bold().FontColor("#E8A020");

                        col.Item().PaddingTop(15).AlignCenter().Text(text =>
                        {
                            text.Span("Por su participación en el evento ").FontSize(13);
                            text.Span($"\"{nombreEvento}\"").FontSize(13).SemiBold();
                            text.Span($", realizado {periodo}.").FontSize(13);
                        });

                        col.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().Height(1).Width(200).Background("#333333");
                                c.Item().PaddingTop(4).Text("Firma autorizada").FontSize(9);
                            });
                        });

                        col.Item().ExtendVertical().AlignBottom().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Código: {codigo}").FontSize(9).FontColor("#666666");
                                c.Item().Text("Verifica la autenticidad de este documento escaneando el código QR.")
                                    .FontSize(8).FontColor("#999999");
                            });
                            row.ConstantItem(80).Image(qrBytes);
                        });
                    });
                });
            });

            documento.GeneratePdf(rutaPdf);
        }

        static ConstanciaDTO MapDTO(EventoConstancia c) => new()
        {
            ID_CONSTANCIA = c.ID_CONSTANCIA,
            ID_EVENTO = c.ID_EVENTO,
            ID_PERSONA = c.ID_PERSONA,
            CODIGO = c.CODIGO,
            ESTADO = c.ESTADO,
            URL_PDF = c.URL_PDF,
            FECHA_GENERACION = c.FECHA_GENERACION,
            USER_GENERACION = c.USER_GENERACION,
            FECHA_ULTIMA_DESCARGA = c.FECHA_ULTIMA_DESCARGA,
            NUM_DESCARGAS = c.NUM_DESCARGAS,
            USER_ANULACION = c.USER_ANULACION,
            FECHA_ANULACION = c.FECHA_ANULACION,
            MOTIVO_ANULACION = c.MOTIVO_ANULACION,
        };
    }
}
