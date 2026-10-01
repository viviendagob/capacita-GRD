using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using ClosedXML.Excel;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using System.Drawing;

namespace CapacitaGRDApi.EndPoints
{
    public static class EventosParticipantesEndpoints
    {
        public static RouteGroupBuilder MapEventosParticipantes(this RouteGroupBuilder group)
        {
            group.MapGet("/", Existe);
            group.MapGet("/eventos", Eventos);
            group.MapPost("/", Agregar);
            group.MapGet("/reporte/{idEvento:int}", Reporte);

            return group;
        }

        
        static async Task<Results<Ok<EventoParticipanteDTO>, NotFound>> Existe(
            IRepositorioEventosParticipantes repositorio
            , IRepositorioEventos repositorioEventos
            , int idPersona
            , int idEvento
            , IMapper mapper
            , IWebHostEnvironment env
            , IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor
        )
        {
            var result = await repositorio.Existe(idEvento, idPersona);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var eventoParticipanteDTO = mapper.Map<EventoParticipanteDTO>(result.FirstOrDefault());
            var resultEvento = await repositorioEventos.Obtener(idEvento);
            var evento = resultEvento.FirstOrDefault();
            if (evento is not null)
            {
                GenerarQR(evento, idPersona, eventoParticipanteDTO, env, configuration, httpContextAccessor);
            }
            return TypedResults.Ok(eventoParticipanteDTO);
        }


        static async Task<Ok<List<EventoParticipanteDTO>>> Eventos(
             IRepositorioEventosParticipantes repositorio
            , IRepositorioEventos repositorioEventos
            , int idPersona
            , IMapper mapper
        )
        {
            var result = await repositorio.Eventos(idPersona);            
            var eventoParticipanteDTO = mapper.Map<List<EventoParticipanteDTO>>(result);

            foreach (var item in eventoParticipanteDTO)
            {
                var resultEvento = await repositorioEventos.Obtener(item.ID_EVENTO);
                var resultEventoDTO = mapper.Map<EventoDTO>(resultEvento.FirstOrDefault());
                item.Evento = resultEventoDTO;
            }

            return TypedResults.Ok(eventoParticipanteDTO);
        }

        static async Task<Results<Ok<EventoParticipanteDTO>, NotFound>> Agregar(
            CrearEventoParticipanteDTO crearEventoParticipanteDTO
           , IRepositorioEventosParticipantes repositorio
           , IRepositorioPersonas repositorioPersonas
           , IRepositorioEventos repositorioEventos
           , IOutputCacheStore outputCacheStore
           , IMapper mapper
            , int idPersona
            , int idEvento
            , IWebHostEnvironment env
            , IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor
           )
        {
            var resultEvento = await repositorioEventos.Obtener(idEvento)!;
            var evento = resultEvento.FirstOrDefault();
            if (evento is null)
            {
                return TypedResults.NotFound();

            }

            var resultPersona = await repositorioPersonas.Obtener(idPersona)!;
            var persona = resultPersona.FirstOrDefault();
            if (persona is null)
            {
                return TypedResults.NotFound();

            }

            var result = await repositorio.Existe(idEvento, idPersona)!;

            if (!result.Any())
            {
                var eventoParticipante = mapper.Map<EventoParticipante>(crearEventoParticipanteDTO);
                await repositorio.Agregar(eventoParticipante);
                var eventoParticipanteDTO = mapper.Map<EventoParticipanteDTO>(eventoParticipante);
                GenerarQR(evento, idPersona, eventoParticipanteDTO, env, configuration, httpContextAccessor);
                return TypedResults.Ok(eventoParticipanteDTO);
            }

            var _eventoParticipante = mapper.Map<EventoParticipante>(crearEventoParticipanteDTO);
            var _eventoParticipanteDTO = mapper.Map<EventoParticipanteDTO>(_eventoParticipante);
            GenerarQR(evento, idPersona, _eventoParticipanteDTO, env, configuration, httpContextAccessor);
            return TypedResults.Ok(_eventoParticipanteDTO);
        }

        // Genera (o reutiliza si ya existe) el QR PERSONAL del participante para este evento —
        // funciona como su "entrada": lo escanea el staff (rol ADMIN en la app) para validar su
        // asistencia, no es el propio participante quien se auto-escanea. Formato:
        // "CAPACITA-GRD:EVENTO:{idEvento}:PERSONA:{idPersona}", un PNG por participante.
        static void GenerarQR(
            Evento evento
            , int idPersona
            , EventoParticipanteDTO dto
            , IWebHostEnvironment env
            , IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor)
        {
            if (evento.GENERA_TICKET != "S")
            {
                return;
            }

            var codigoQR = $"CAPACITA-GRD:EVENTO:{evento.ID_EVENTO}:PERSONA:{idPersona}";
            var carpetaEventos = configuration.GetSection("folferEventos").Value;
            var folderEventoQR = Path.Combine(env.WebRootPath, carpetaEventos, evento.COD_EVENTO, "QR");
            if (!Directory.Exists(folderEventoQR))
            {
                Directory.CreateDirectory(folderEventoQR);
            }

            var nombreArchivo = $"persona_{idPersona}.png";
            var fileQR = Path.Combine(folderEventoQR, nombreArchivo);
            if (!File.Exists(fileQR))
            {
                var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                var qrCode = qrEncoder.Encode(codigoQR);
                var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                using var stream = new FileStream(fileQR, FileMode.Create);
                renderer.WriteToStream(qrCode.Matrix, imageFormat: System.Drawing.Imaging.ImageFormat.Png, stream);
            }

            var baseUrl = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            dto.CODIGO_QR = codigoQR;
            dto.QR_URL = Path.Combine(baseUrl, carpetaEventos, evento.COD_EVENTO, "QR", nombreArchivo).Replace("\\", "/");
        }

        static async Task<FileContentHttpResult> Reporte(
            int idEvento
            , IRepositorioEventosParticipantes repositorio)
        {
            var inscritos = await repositorio.ReporteInscritos(idEvento);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Inscritos");

            ws.Cell(1, 1).Value = "Tipo Documento";
            ws.Cell(1, 2).Value = "Num Documento";
            ws.Cell(1, 3).Value = "Nombres";
            ws.Cell(1, 4).Value = "Apellido Paterno";
            ws.Cell(1, 5).Value = "Apellido Materno";
            ws.Cell(1, 6).Value = "Email";
            ws.Cell(1, 7).Value = "Celular";
            ws.Cell(1, 8).Value = "Fecha de Inscripción";
            ws.Range(1, 1, 1, 8).Style.Font.Bold = true;

            var fila = 2;
            foreach (var inscrito in inscritos)
            {
                ws.Cell(fila, 1).Value = inscrito.TIPO_DOCUMENTO;
                ws.Cell(fila, 2).Value = inscrito.NUM_DOCUMENTO;
                ws.Cell(fila, 3).Value = inscrito.NOMBRES;
                ws.Cell(fila, 4).Value = inscrito.APELLIDO_PATERNO;
                ws.Cell(fila, 5).Value = inscrito.APELLIDO_MATERNO;
                ws.Cell(fila, 6).Value = inscrito.EMAIL;
                ws.Cell(fila, 7).Value = inscrito.CELULAR;
                ws.Cell(fila, 8).Value = inscrito.FECHA_REG.ToString("dd/MM/yyyy HH:mm");
                fila++;
            }

            ws.Columns(1, 8).Width = 18;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return TypedResults.Bytes(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reporte_evento.xlsx");
        }

    }
}
