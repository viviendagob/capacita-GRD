using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using CapacitaGRDApi.Util;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;

namespace CapacitaGRDApi.EndPoints
{
    public static class RegistrosEndpoints
    {
        public static RouteGroupBuilder MapRegistro(this RouteGroupBuilder group)
        {
            group.MapGet("/eventos", Listar);
            group.MapGet("/validar", Validar);
            group.MapPost("/agregar", Agregar);            
            group.MapPost("/asistencia", Asistencia);

            return group;
        }

        static async Task<Ok<List<EventoDTO>>> Listar(
              IRepositorioEventos repositorio
            , IRepositorioEstados repositorioEstado
            , IRepositorioModalidad repositorioModalidad
            , IRepositorioTipoEventos repositorioTipoEvento
            , IRepositorioDistritos repositorioDistritos
            , IMapper mapper
            , IOutputCacheStore outputCacheStore
            )
        {
            var eventos = await repositorio.Listar();
            var listarEventosActivos = eventos.Where(evento => evento.ID_ESTADO == Constantes.estadoEventoActivo);
            List<EventoDTO> eventosDTO = []; 

            foreach (var evento in listarEventosActivos)
            {
                var tipoevento = await repositorioTipoEvento.Obtener(evento.ID_TIPO_EVENTO);
                var tipoeventoDTO = mapper.Map<TipoEventoDTO>(tipoevento.FirstOrDefault());
                 
                var estado = await repositorioEstado.Obtener(evento.ID_ESTADO);
                var estadoDTO = mapper.Map<EstadoDTO>(estado.FirstOrDefault());
               
                var modalidad = await repositorioModalidad.Obtener(evento.ID_MODALIDAD);
                var modalidadDTO = mapper.Map<ModalidadDTO>(modalidad.FirstOrDefault());
               
                var distrito = await repositorioDistritos.Obtener(evento.ID_UBIGEO);
                var distritoDTO = mapper.Map<DistritoDTO>(distrito.FirstOrDefault());
                 
                eventosDTO.Add(new EventoDTO
                {
                    ID_EVENTO = evento.ID_EVENTO,
                    BANNER = evento.BANNER,
                    COD_EVENTO = evento.COD_EVENTO,
                    DESCRIPCION = evento.DESCRIPCION,
                    ENLACE_WHATSAPP = evento.ENLACE_WHATSAPP,
                    ESTADO = estadoDTO,
                    ID_MODALIDAD = evento.ID_MODALIDAD,
                    MODALIDAD = modalidadDTO,
                    ID_TIPO_EVENTO = evento.ID_TIPO_EVENTO,
                    TIPO_EVENTO = tipoeventoDTO,
                    ID_UBIGEO = evento.ID_UBIGEO,
                    DISTRITO = distritoDTO,
                    FECHA_INICIO = evento.FECHA_INICIO,
                    FECHA_FIN = evento.FECHA_FIN,   
                    HORA_INICIO = evento.HORA_INICIO,
                    HORA_FIN = evento.HORA_FIN,
                    NOMBRE = evento.NOMBRE,
                    NOMBRE_LUGAR = evento.NOMBRE_LUGAR,
                    NUM_PARTICIPANTES = evento.NUM_PARTICIPANTES,
                    RED_SOCIAL = evento.RED_SOCIAL,
                });
            }

            return TypedResults.Ok(eventosDTO);
        }

        static async Task<Results<Ok<Response<dynamic>>, NotFound<Response<dynamic>>>> Agregar(
              int idEvento
            , int tipoDocumento
            , int idModalidad
            , string documento
            , string departamento
            , string provincia
            , string distrito
            , CrearPersonaDTO crearPersonaDTO
            , IRepositorioEventos repositorioEventos
            , IRepositorioPIDE repositorioPIDE
            , IRepositorioEventosFechas repositorioEventosFechas
            , IRepositorioEventosAsistencias repositorioEventosAsistencias
            , IRepositorioEventosParticipantes repositorioEventosParticipantes
            , IRepositorioPersonas repositorioPersonas
            , IRepositorioPersonasData repositorioPersonasData
            , IMapper mapper
            , IOutputCacheStore outputCacheStore
            , IWebHostEnvironment env
            , IConfiguration Configuration
            , IEmailSender emailSender
            , IHttpContextAccessor httpContextAccessor
             )
        {
            var response = new Response<dynamic>();

            var existeEvento = await repositorioEventos.Obtener(idEvento);
            if (!existeEvento.Any())
            {
                response.titulo = "Advertencia";
                response.mensaje = "El evento no es valido";
                return TypedResults.NotFound(response);
            }
            else
            {
                if (existeEvento.FirstOrDefault().ID_ESTADO != Constantes.estadoEventoActivo)
                {
                    response.titulo = "Advertencia";
                    response.mensaje = "El evento no se encuentra disponible";
                    return TypedResults.NotFound(response);
                }
            }

            //
            int idPersona = 0;
            if (tipoDocumento == 1)
            {
                var validaDNI = await repositorioPIDE.ValidaDNI(documento);
                if (validaDNI is null)
                {
                    response.titulo = "Advertencia";
                    response.mensaje = "El documento de identidad ingresado no es valido";
                    return TypedResults.NotFound(response);
                }
                else
                {
                    var ubigeo = ((departamento == "CALLAO" ? "" : departamento) + "/" + provincia + "/" + distrito);
                    if (validaDNI.COD_PERSONA == ubigeo)
                    {
                        var crearPersona = await repositorioPersonas.Existe(crearPersonaDTO.ID_TIPO_DOCUMENTO, crearPersonaDTO.NUM_DOCUMENTO);
                        if (!crearPersona.Any())
                        {
                            var crearRegistroPersona = mapper.Map<Persona>(crearPersonaDTO);
                            idPersona = await repositorioPersonas.Agregar(crearRegistroPersona);
                        }
                        else {
                            idPersona = crearPersona.FirstOrDefault().ID_PERSONA;
                        }
                    }
                    else
                    {
                        response.titulo = "Advertencia";
                        response.mensaje = "Los datos de la persona no son validos";
                        return TypedResults.NotFound(response);
                    }
                }
            }

            var personaDatos = await repositorioPersonas.Obtener(idPersona);
            Persona persona = personaDatos.FirstOrDefault();

            var crearPersonaData = mapper.Map<PersonaData>(crearPersonaDTO.CrearPersonaDataDTO);
            crearPersonaData.ID_PERSONA = idPersona;
             
            var existeEventoParticipante = await repositorioEventosParticipantes.Existe(idEvento, idPersona);
            if (existeEventoParticipante.Any())
            {
                 response.titulo = "Advertencia";
                response.mensaje = "El participante ya se encuentra registrado";
                return TypedResults.NotFound(response);
            }

            var lastPersonaData = await repositorioPersonasData.Agregar(crearPersonaData);
            var eventoParticipante = new EventoParticipante()
            {
                ID_EVENTO = idEvento,
                ID_PERSONA_DATA = lastPersonaData,
                ID_MODALIDAD = idModalidad,
                LATITUD = 0,
                LONGITUD = 0,
                ID_PERSONA = idPersona
            };
            await repositorioEventosParticipantes.Agregar(eventoParticipante);

            var evento = existeEvento.FirstOrDefault();
            string tickedPDF = "";
            string urlTickedPDF = "";
            string codigoQR = "";
            var nombrePDF = Guid.NewGuid() + ".pdf";
            if (evento.GENERA_TICKET == "S")
            {
                var carpetaEventos = Configuration.GetSection("folferEventos").Value;
                var folderEventos = Path.Combine(env.WebRootPath, carpetaEventos);
                if (!Directory.Exists(folderEventos))
                {
                    Directory.CreateDirectory(folderEventos);
                }
                
                var folderEvento = Path.Combine(folderEventos, evento.COD_EVENTO);
                if (!Directory.Exists(folderEvento))
                {
                    Directory.CreateDirectory(folderEvento);
                }

                var folderEventoQR = Path.Combine(folderEvento, "QR");
                if (!Directory.Exists(folderEventoQR))
                {
                    Directory.CreateDirectory(folderEventoQR);
                }
                 
                codigoQR = evento.COD_EVENTO + "_" + persona.COD_PERSONA;

                var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                var qrCode = qrEncoder.Encode(codigoQR);
                var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                var fileQR = Path.Combine(folderEventoQR, codigoQR) + ".png";

                using var stream = new FileStream(fileQR, FileMode.Create);
                renderer.WriteToStream(qrCode.Matrix, imageFormat: System.Drawing.Imaging.ImageFormat.Png, stream);
                 
                var folderEventoTicket = Path.Combine(folderEvento, "ticket");
                if (!Directory.Exists(folderEventoTicket))
                {
                    Directory.CreateDirectory(folderEventoTicket);
                } 
                
                var folderEventoBanner = Path.Combine(folderEvento, "banner");
                var imagenBanner = Path.Combine(folderEventoBanner, existeEvento.FirstOrDefault().BANNER);
                var fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                
                tickedPDF = Path.Combine(folderEventoTicket, nombrePDF );
                var ticket = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(10);
                        page.Size(88, 190, QuestPDF.Infrastructure.Unit.Millimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(f => f.FontSize(10).Italic());
                        page.Content().Column(
                            column =>
                            {
                                column.Item()
                                .Image(imagenBanner)
                                .WithCompressionQuality(ImageCompressionQuality.Medium);

                                column.Item()
                                .Text("Hola, " + persona.NOMBRES);

                                column.Item()                            
                                .Text("Tu identificador QR es,");

                                column.Item()
                                .PaddingTop(10)
                                .PaddingLeft(50)
                                .PaddingRight(50)
                                .PaddingBottom(20)
                                .Image(fileQR)
                                .WithCompressionQuality(ImageCompressionQuality.Medium);

                                column.Item()
                                .Text("Lugar: " + evento.NOMBRE_LUGAR);

                                column.Item()
                                .Text("Día(s): " + evento.FECHA_INICIO.ToString("dd/MM/yyyy") + " al " + evento.FECHA_FIN.ToString("dd/MM/yyyy"));

                                column.Item()
                                .Text("Horario: " + evento.HORA_INICIO.ToString("HH:mm") + " al " + evento.HORA_FIN.ToString("HH:mm"));


                                column.Item()
                               .PaddingTop(10)
                               .Text("Asegurate de llevar este QR para acceder a las actividades del evento.");
                                
                                column.Item()
                                .PaddingTop(10)
                                .Text("Lima, " + fecha);
                            }
                        );                         
                    });
                  
                });

                ticket.GeneratePdf(tickedPDF);
                var url  = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
                var archivo = carpetaEventos + "/" + evento.COD_EVENTO + "/ticket/" + nombrePDF;
                urlTickedPDF = Path.Combine(url, archivo).Replace("\\", "/");

            }

            if (evento.ENVIA_CORREO == "S")
            {

                var adjunto = evento.GENERA_TICKET;
                var asunto = "Bienvenidos al " + evento.NOMBRE;
                var mensaje = "<!DOCTYPE html>";
                mensaje += "<html>";
                mensaje += "<head>";
                mensaje += "<style>";
                mensaje += "p{";
                mensaje += "font -family: \"Sans -serif\", arial, verdana;";
                mensaje += "font-size: 13px;";
                mensaje += "}";
                mensaje += "</style>";
                mensaje += "</head>";
                mensaje += "<body>";
                mensaje += "<p><b>Estimado(a) </b>" + persona.NOMBRES + "</p>";
                mensaje += "<p>Usted ha sido registrado satisfactoriamente.</p>";
                mensaje += "<p style=\"line-height: 0.5;\">&nbsp;</p>";
                mensaje += "<p style = \"line-height: 0.5;\" ><b> Evento:</b>" + evento.NOMBRE + "</p >";

                if (adjunto == "S")
                {
                    mensaje += "<p style=\"line -height: 0.5;\">&nbsp;</p>";
                    mensaje += "<p>Adjunto encontrarás el código QR de inscripción para el evento. Por favor, asegúrate de llevar este QR contigo al evento, ya que te permitirá acceder a todas las actividades programadas y facilitará tu registro de asistencia.</p>";                    
                }

                 mensaje += "<p>Si tienes alguna pregunta o necesitas más información sobre el evento, no dudes en ponerte en contacto al correo electrónico <a href=\"mailto:ugerdes@vivienda.gob.pe\">ugerdes@vivienda.gob.pe</a>. Esperamos contar con tu participación y que disfrutes de esta experiencia.</p>";
                 mensaje += "<p>&nbsp;</p>";
                 mensaje += "<p style=\"line -height: 0.5;\"><b>Saludos cordiales,</b></p>";
                 mensaje += "<p style=\"line -height: 0.5;\">Equipo de la UGERDES | Programa Nuestra Ciudades | Ministerio de Vivienda, Construcción y Saneamiento</p>";
                 mensaje += "</body>";
                 mensaje += "</html>";

                //
                //<p style="line-height: 0.5;"><b>Modalidad:</b> ' . $modalidad . '</p>
                //<p style="line-height: 0.5;"><b>Fecha:</b> ' . $periodo . '</p>
                //<p style="line-height: 0.5;"><b>Horario:</b> ' . $horario . '</p>
                //'.($modalidad == 'VIRTUAL' ? " " : "<p style='line-height: 0.5;'><b>Lugar:</b> $nomlugar </p>").'
                //'.($modalidad == 'VIRTUAL' ? "<p style='line-height: 0.5;'><b>Url:</b> $enlace </p>" : " ").'
                //'.($redsocial != '' ? "<p style='line-height: 0.5;'><b>Únete al grupo de Whatsapp del evento:</b> <a href='$redsocial' title='Ir al evento' target='_blank'>$redsocial</a>  </p>" : " ").'
                //<p style="line-height: 0.5;">&nbsp;</p>		

                //<p>Adjunto encontrarás el código QR de inscripción para el evento. Por favor, asegúrate de llevar este QR contigo al evento, ya que te permitirá acceder a todas las actividades programadas y facilitará tu registro de asistencia.</p>			
                //<p>Si tienes alguna pregunta o necesitas más información sobre el evento, no dudes en ponerte en contacto al correo electrónico <a href="mailto:ugerdes@vivienda.gob.pe">ugerdes@vivienda.gob.pe</a>. Esperamos contar con tu participación y que disfrutes de esta experiencia.</p>			

                await emailSender.SendEmailAsync(persona.EMAIL, asunto, mensaje, (adjunto == "S" ? tickedPDF : "")).ConfigureAwait(false);

            }

            response.titulo = "Bien";
            response.mensaje = "Participante registrado correctamente";
            response.data = urlTickedPDF;
            return TypedResults.Ok(response);
        }

        static async Task<Results<Ok<Response<dynamic>>, NotFound<Response<dynamic>>>> Asistencia(
              int idEvento
            , int tipoDocumento
            , int idModalidad
            , string documento
            , string departamento
            , string provincia
            , string distrito
            , CrearPersonaDTO crearPersonaDTO
            , IRepositorioEventos repositorioEventos
            , IRepositorioPIDE repositorioPIDE
            , IRepositorioEventosFechas repositorioEventosFechas
            , IRepositorioEventosAsistencias repositorioEventosAsistencias
            , IRepositorioEventosParticipantes repositorioEventosParticipantes
            , IRepositorioPersonas repositorioPersonas
            , IRepositorioPersonasData repositorioPersonasData
            , IMapper mapper
           , IOutputCacheStore outputCacheStore)
        {
            var response = new Response<dynamic>();
            DateTime fecha  = DateTime.Now.Date;

            var existeEvento = await repositorioEventos.Obtener(idEvento);
            if (!existeEvento.Any())
            {
                response.titulo = "Advertencia";
                response.mensaje = "El evento no es valido";
                return TypedResults.NotFound(response);
            }

            var fechaEvento = await repositorioEventosFechas.Obtener(idEvento, fecha);
            if (!fechaEvento.Any())
            {
                response.titulo = "Advertencia";
                response.mensaje = "El evento se encuentra fuera de fecha";

                return TypedResults.NotFound(response);
            }

            Persona persona = null;
            int idPersona = 0;
            if (tipoDocumento == 1)
            {
                persona = await repositorioPIDE.ValidaDNI(documento);
                if (persona is null)
                {
                    response.titulo = "Advertencia";
                    response.mensaje = "El documento de identidad ingresado no es valido";
                    return TypedResults.NotFound(response);
                }
                else
                {
                    var ubigeo = ((departamento == "CALLAO" ? "" : departamento) + "/" + provincia + "/" + distrito);
                    if (persona.COD_PERSONA == ubigeo)
                    {
                        var crearPersona = await repositorioPersonas.Existe(crearPersonaDTO.ID_TIPO_DOCUMENTO, crearPersonaDTO.NUM_DOCUMENTO);
                        if (!crearPersona.Any())
                        {
                            var crearRegistroPersona = mapper.Map<Persona>(crearPersonaDTO);
                            idPersona = await repositorioPersonas.Agregar(crearRegistroPersona);
                        }
                        else
                        {
                            idPersona = crearPersona.FirstOrDefault().ID_PERSONA;
                        }
                    }
                    else
                    {
                        response.titulo = "Advertencia";
                        response.mensaje = "Los datos ingresados no son validos";
                        return TypedResults.NotFound(response);
                    }
                }
            }

            var crearPersonaData = mapper.Map<PersonaData>(crearPersonaDTO.CrearPersonaDataDTO);
            crearPersonaData.ID_PERSONA = idPersona;
             
            var existeEventoParticipante = await repositorioEventosParticipantes.Existe(idEvento, idPersona);
            if (!existeEventoParticipante.Any())
            {
                var lastPersonaData = await repositorioPersonasData.Agregar(crearPersonaData);
                var eventoParticipante = new EventoParticipante()
                {
                    ID_EVENTO = idEvento,
                    ID_PERSONA_DATA = lastPersonaData,
                    ID_MODALIDAD = idModalidad,
                    LATITUD = 0,
                    LONGITUD = 0,
                    ID_PERSONA = idPersona
                };
                await repositorioEventosParticipantes.Agregar(eventoParticipante);
                //Genero Ticket
            }
             
            EventoAsistencia eventoAsistencia = new EventoAsistencia()
            {
                ID_EVENTO = idEvento,
                ID_PERSONA = idPersona,
                FECHA = fecha,
                ID_MODALIDAD = idModalidad
            };

            var existeAsistencia = await repositorioEventosAsistencias.Existe(eventoAsistencia);
            if (!existeAsistencia.Any())
            {
                await repositorioEventosAsistencias.Agregar(eventoAsistencia);
                
                response.titulo = "Bien";
                response.mensaje = "Asistencia guardada correctamente";
                return TypedResults.Ok(response);
            }

            response.titulo = "Bien";
            response.mensaje = "Asistencia guardada correctamente";
            return TypedResults.Ok(response);

        }

        static async Task<Response<Persona>> Validar(
              int tipo
            , string documento
            , string departamento
            , string provincia
            , string distrito
            , IRepositorioPIDE repositorio
            , IRepositorioPersonas repositorioPersonas
            , IOutputCacheStore outputCacheStore
            )
        {
            var response = new Response<Persona>();
            Persona personaResult = new();

            if (tipo == 1)
            {
                var persona = await repositorio.ValidaDNI(documento);

                if (persona is null)
                {
                    personaResult = null;

                    response.success = false;
                    response.titulo = "Upss";
                    response.mensaje = "Los datos de validación no son los correctos";

                }
                else
                {
                    var ubigeo = ((departamento == "CALLAO" ? "" : departamento) + "/" + provincia + "/" + distrito);

                    if (persona.COD_PERSONA == ubigeo)
                    {
                        var datos_persona = await repositorioPersonas.Existe(tipo, documento);
                        
                        if (datos_persona.Any())
                        {
                            if (datos_persona.FirstOrDefault().VALIDADO_PIDE  == "S")
                            {
                                personaResult = datos_persona.FirstOrDefault();
                            }
                            else
                            {
                                personaResult = persona;
                            }                            
                        }
                        else
                        {
                            personaResult = persona;
                        }

                        response.success = true;
                        response.titulo = "Bien";
                        response.mensaje = "Datos validados correctamente";
                    }
                    else
                    {
                        personaResult = null;

                        response.success = false;
                        response.titulo = "Upss";
                        response.mensaje = "Los datos de validación no son los correctos";

                    }
                } 

            }

            if (tipo == 2)
            {
                var persona = await repositorio.ValidaCE(documento);
                if (persona is null)
                {
                    personaResult = null;

                    response.success = false;
                    response.titulo = "Upss";
                    response.mensaje = "Los datos de validación no son los correctos";

                }
                else
                {
                    var datos_persona = await repositorioPersonas.Existe(tipo, documento);
                    personaResult = datos_persona.FirstOrDefault();

                    response.success = true;
                    response.titulo = "Bien";
                    response.mensaje = "Datos validados correctamente";
                }
            }

            if (tipo == 3)
            {
                var result = await repositorioPersonas.Existe(tipo, documento);
                if (!result.Any())
                {
                    personaResult = null;

                    response.success = false;
                    response.titulo = "Upss";
                    response.mensaje = "Los datos de validación no son los correctos";

                }
                else
                {
                    personaResult = result.FirstOrDefault();

                    response.success = true;
                    response.titulo = "Bien";
                    response.mensaje = "Datos validados correctamente";
                }

            }

            response.data = personaResult;

                return response;

        }
    }
}
