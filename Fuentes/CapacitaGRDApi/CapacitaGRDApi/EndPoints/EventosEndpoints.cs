using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{

    public static class EventosEndpoints
    {
        public static RouteGroupBuilder MapEventos(this RouteGroupBuilder group)
        {
         
            group.MapGet("/", Listar);
            group.MapGet("/estadisticas", Estadisticas);
            group.MapGet("/{id:int}", Obtener);
            group.MapPost("/", Agregar);
            group.MapPost("/paginado", Paginar);
            group.MapPut("/", Actualizar);
            group.MapPut("/fechas", ActualizarFechas);
            group.MapDelete("/", Eliminar);

            return group;
        }

        static async Task<Ok<List<EventoDTO>>> Listar(
              IRepositorioEventos repositorio
            , IRepositorioEstados repositorioEstado
            , IRepositorioModalidad repositorioModalidad
            , IRepositorioTipoEventos repositorioTipoEvento
        , IRepositorioDistritos repositorioDistritos
            , IMapper mapper)
        {

            var eventos = await repositorio.Listar();
            List<EventoDTO> eventosDTO = [];

            foreach (var evento in eventos)
            {
                var tipoevento = await repositorioTipoEvento.Obtener(evento.ID_TIPO_EVENTO);
                var tipoeventoDTO = mapper.Map<TipoEventoDTO>(tipoevento.FirstOrDefault());

                var estado = await repositorioEstado.Obtener(evento.ID_ESTADO);
                var estadoDTO = mapper.Map<EstadoDTO>(estado.FirstOrDefault());

                var modalidad = await repositorioModalidad.Obtener(evento.ID_MODALIDAD);
                var modalidadDTO = mapper.Map<ModalidadDTO>(modalidad.FirstOrDefault());

                var distrito = await repositorioDistritos.Obtener(evento.ID_UBIGEO);
                var distritoDTO = mapper.Map<DistritoDTO>(distrito.FirstOrDefault());

                var eventoDTO = mapper.Map<EventoDTO>(evento);
                eventoDTO.TIPO_EVENTO = tipoeventoDTO;
                eventoDTO.ESTADO = estadoDTO;
                eventoDTO.MODALIDAD = modalidadDTO;
                eventoDTO.DISTRITO = distritoDTO;

                eventosDTO.Add(eventoDTO);
            }

            return TypedResults.Ok(eventosDTO);
        }

        static async Task<Ok<EventoEstadisticaDTO>> Estadisticas(
            IRepositorioEventos repositorio)
        {
            var estadisticas = await repositorio.Estadisticas();
            return TypedResults.Ok(estadisticas);
        }

    static async Task<Ok<PaginadorDTO<EventoDTO>>> Paginar(
     BusquedaEventoDTO filtro
    , IRepositorioEventos repositorio
    , IRepositorioEstados repositorioEstado
    , IRepositorioModalidad repositorioModalidad
    , IRepositorioTipoEventos repositorioTipoEvento
    , IRepositorioDistritos repositorioDistritos
    , IMapper mapper)
        {
            var paginado = filtro.Filtro;
            var buscarModalidad = filtro.Modalidad;
            var buscartipo = filtro.Tipo;            
            var buscarEstado = filtro.Estado;

            int pagActual = paginado.start;
            int pagDesde = (pagActual == 0) ? 1 : (pagActual + 1);
            int pagHasta = pagActual + paginado.length;
            var inxColumn = paginado.order[0].column;
            var orderColumn = paginado.order[0].dir;
            var orderBy = paginado.columns[inxColumn].name + ' ' + orderColumn;

            var where = " ";
            if (buscarModalidad != 0)
            {
                where += " AND ID_MODALIDAD = " + buscarModalidad;
            }

            if (buscartipo != 0)
            {
                where += " AND ID_TIPO_EVENTO = " + buscartipo;
            }

            if (buscarEstado != 0)
            {
                where += " AND ID_ESTADO = " + buscarEstado;
            }

            Filter filter = new()
            {
                Limit = "  FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var eventos = await repositorio.Paginar(filter);
            List<EventoDTO> eventosDTO = [];

            foreach (var evento in eventos)
            {
                var tipoevento = await repositorioTipoEvento.Obtener(evento.ID_TIPO_EVENTO);
                var tipoeventoDTO = mapper.Map<TipoEventoDTO>(tipoevento.FirstOrDefault());

                var estado = await repositorioEstado.Obtener(evento.ID_ESTADO);
                var estadoDTO = mapper.Map<EstadoDTO>(estado.FirstOrDefault());

                var modalidad = await repositorioModalidad.Obtener(evento.ID_MODALIDAD);
                var modalidadDTO = mapper.Map<ModalidadDTO>(modalidad.FirstOrDefault());

                var distrito = await repositorioDistritos.Obtener(evento.ID_UBIGEO);
                var distritoDTO = mapper.Map<DistritoDTO>(distrito.FirstOrDefault());

                var eventoDTO = mapper.Map<EventoDTO>(evento);
                eventoDTO.TIPO_EVENTO = tipoeventoDTO;
                eventoDTO.ESTADO    = estadoDTO;
                eventoDTO.MODALIDAD = modalidadDTO;
                eventoDTO.DISTRITO = distritoDTO;

                eventosDTO.Add(eventoDTO);             
            }

            var response = new PaginadorDTO<EventoDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (eventos.Any() ? eventos.FirstOrDefault().REGISTROS : 0)  ,
                recordsTotal = (eventos.Any() ? eventos.FirstOrDefault().REGISTROS : 0),
                data = eventosDTO
            };

            return TypedResults.Ok(response);
        }


        static async Task<Results<Ok<EventoDTO>, NotFound>> Obtener(
                IRepositorioEventos repositorio
            , IRepositorioEventosFechas repositorioEventosFechas
            , IRepositorioEstados repositorioEstado
            , IRepositorioModalidad repositorioModalidad
            , IRepositorioTipoEventos repositorioTipoEvento
            , IRepositorioDistritos repositorioDistritos
            , IRepositorioEventosDocumentos repositorioDocumentos
            , int id
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
                return TypedResults.NotFound();

            if (!result.Any())
                return TypedResults.NotFound();

            var evento = result.FirstOrDefault(); 

            var tipoevento = await repositorioTipoEvento.Obtener(evento.ID_TIPO_EVENTO);
            var tipoeventoDTO = mapper.Map<TipoEventoDTO>(tipoevento.FirstOrDefault());

            var estado = await repositorioEstado.Obtener(evento.ID_ESTADO);
            var estadoDTO = mapper.Map<EstadoDTO>(estado.FirstOrDefault());

            var modalidad = await repositorioModalidad.Obtener(evento.ID_MODALIDAD);
            var modalidadDTO = mapper.Map<ModalidadDTO>(modalidad.FirstOrDefault());

            var distrito = await repositorioDistritos.Obtener(evento.ID_UBIGEO);
            var distritoDTO = mapper.Map<DistritoDTO>(distrito.FirstOrDefault());

            var eventoFechas = await repositorioEventosFechas.Listar(id);
            var eventoFechasDTO = mapper.Map<List<EventoFechaDTO>>(eventoFechas);

            var eventoDocumentos = await repositorioDocumentos.Listar(id);
            var eventoDocumentosDTO = mapper.Map<List<EventoDocumentoDTO>>(eventoDocumentos);

            var eventoDTO = mapper.Map<EventoDTO>(evento);
            eventoDTO.TIPO_EVENTO = tipoeventoDTO;
            eventoDTO.ESTADO = estadoDTO;
            eventoDTO.MODALIDAD = modalidadDTO;
            eventoDTO.DISTRITO = distritoDTO;
            eventoDTO.FECHAS = eventoFechasDTO;
            eventoDTO.Documentos= eventoDocumentosDTO;


            return TypedResults.Ok(eventoDTO);
        }

        static async Task<Created<EventoDTO>> Agregar(
            CrearEventoDTO crearEventoDTO
            , IRepositorioEventos repositorio
            , IRepositorioEventosFechas repositorioEventosFechas
            , IRepositorioEstados repositorioEstado
            , IRepositorioModalidad repositorioModalidad
            , IRepositorioTipoEventos repositorioTipoEvento
            , IRepositorioDistritos repositorioDistritos
            , IRepositorioEventosDocumentos repositorioEventosDocumentos
            , IMapper mapper
            , IOutputCacheStore outputCacheStore
            , IWebHostEnvironment env
            , IConfiguration Configuration
           )
        {
            var banerImagenFormato = Guid.NewGuid() + ".png";
            var evento = new Evento
            {
                NOMBRE = crearEventoDTO.NOMBRE,
                FECHA_INICIO = crearEventoDTO.FECHA_INICIO,
                FECHA_FIN = crearEventoDTO.FECHA_FIN,
                NOMBRE_LUGAR = crearEventoDTO.NOMBRE_LUGAR,
                ID_ESTADO = crearEventoDTO.ID_ESTADO,
                ID_UBIGEO = crearEventoDTO.ID_UBIGEO,
                BANNER = banerImagenFormato,
                FORMATO = banerImagenFormato,
                NUM_PARTICIPANTES = crearEventoDTO.NUM_PARTICIPANTES,
                DESCRIPCION = crearEventoDTO.DESCRIPCION,
                ENLACE_WHATSAPP = crearEventoDTO.ENLACE_WHATSAPP,
                HORA_INICIO = TimeOnly.Parse(crearEventoDTO.HORA_INICIO!),
                HORA_FIN = TimeOnly.Parse(crearEventoDTO.HORA_FIN!),
                RED_SOCIAL = crearEventoDTO.RED_SOCIAL,
                ID_TIPO_EVENTO = crearEventoDTO.ID_TIPO_EVENTO,
                ID_MODALIDAD = crearEventoDTO.ID_MODALIDAD,
                GENERA_TICKET = crearEventoDTO.GENERA_TICKET,
                ENVIA_CORREO = crearEventoDTO.ENVIA_CORREO,
                REQUIERE_DOCUMENTO = crearEventoDTO.REQUIERE_DOCUMENTO,
                USER_REG = "JFLORES"
            };

            var id = await repositorio.Agregar(evento);

            if (crearEventoDTO.FECHAS is not null)
            {
                foreach (CrearEventoFechaDTO item in crearEventoDTO.FECHAS)
                {
                    EventoFecha eventoFecha = new EventoFecha
                    {
                        ES_CUESTIONARIO = item.ES_CUESTIONARIO,
                        ES_ENCUESTA = item.ES_ENCUESTA,
                        FECHA = item.FECHA,
                        HORA_FIN = item.HORA_FIN,
                        HORA_INICIO = item.HORA_INICIO,
                        ID_CUESTIONARIO = item.ID_CUESTIONARIO,
                        ID_ENCUESTA = item.ID_ENCUESTA,
                        ID_EVENTO = id
                    };

                    await repositorioEventosFechas.Agregar(eventoFecha);
                }
            }


            var lastEvento = await repositorio.Obtener(id);

            if (crearEventoDTO.BANNER is not null)
            {
                if (crearEventoDTO.BANNER.Trim() != "")
                {
                    var carpetaEventos = Configuration.GetSection("folferEventos").Value;
                    var folderEventos = Path.Combine(env.WebRootPath, carpetaEventos);
                    if (!Directory.Exists(folderEventos))
                    {
                        Directory.CreateDirectory(folderEventos);
                    }

                    var folderEvento = Path.Combine(folderEventos, lastEvento.FirstOrDefault().COD_EVENTO);
                    if (!Directory.Exists(folderEvento))
                    {
                        Directory.CreateDirectory(folderEvento);
                    }

                    var folderEventoBanner = Path.Combine(folderEvento, "banner");
                    if (!Directory.Exists(folderEventoBanner))
                    {
                        Directory.CreateDirectory(folderEventoBanner);
                    }

                    var banner = Path.Combine(folderEventoBanner, banerImagenFormato);
                    var bytess = Convert.FromBase64String(crearEventoDTO.BANNER);
                    using var imageFile = new FileStream(banner, FileMode.Create);
                    imageFile.Write(bytess, 0, bytess.Length);
                    imageFile.Flush();

                    evento.BANNER = banerImagenFormato;
                }
            }

            if (crearEventoDTO.FORMATO is not null)
            {
                if (crearEventoDTO.FORMATO.Trim() != "")
                {
                    var carpetaEventos = Configuration.GetSection("folferEventos").Value;
                    var folderEventos = Path.Combine(env.WebRootPath, carpetaEventos);
                    if (!Directory.Exists(folderEventos))
                    {
                        Directory.CreateDirectory(folderEventos);
                    }

                    var folderEvento = Path.Combine(folderEventos, lastEvento.FirstOrDefault().COD_EVENTO);
                    if (!Directory.Exists(folderEvento))
                    {
                        Directory.CreateDirectory(folderEvento);
                    }

                    var folderEventoBanner = Path.Combine(folderEvento, "formato");
                    if (!Directory.Exists(folderEventoBanner))
                    {
                        Directory.CreateDirectory(folderEventoBanner);
                    }

                    var banner = Path.Combine(folderEventoBanner, banerImagenFormato);
                    var bytess = Convert.FromBase64String(crearEventoDTO.FORMATO);
                    using var imageFile = new FileStream(banner, FileMode.Create);
                    imageFile.Write(bytess, 0, bytess.Length);
                    imageFile.Flush();

                    evento.FORMATO = banerImagenFormato;
                }
            }

            if (crearEventoDTO.Documentos is not null)
            {
           
                foreach (var item in crearEventoDTO.Documentos)
                {
                    EventoDocumento eventoDocumento = new()
                    {
                        ID_EVENTO = id,
                        ID_TIPO_DOCUMENTO_REQUERIDO = item
                    };

                    await repositorioEventosDocumentos.Agregar(eventoDocumento);
                }
            }
           

            var tipoevento = await repositorioTipoEvento.Obtener(evento.ID_TIPO_EVENTO);
            var tipoeventoDTO = mapper.Map<TipoEventoDTO>(tipoevento.FirstOrDefault());

            var estado = await repositorioEstado.Obtener(evento.ID_ESTADO);
            var estadoDTO = mapper.Map<EstadoDTO>(estado.FirstOrDefault());

            var modalidad = await repositorioModalidad.Obtener(evento.ID_MODALIDAD);
            var modalidadDTO = mapper.Map<ModalidadDTO>(modalidad.FirstOrDefault());

            var distrito = await repositorioDistritos.Obtener(evento.ID_UBIGEO);
            var distritoDTO = mapper.Map<DistritoDTO>(distrito.FirstOrDefault());

            var eventoDocumentos = await repositorioEventosDocumentos.Listar(id);
            var eventoDocumentosDTO = mapper.Map<List<EventoDocumentoDTO>>(eventoDocumentos);

            var eventoDTO = new EventoDTO
            {
                COD_EVENTO = lastEvento.FirstOrDefault().COD_EVENTO,  
                NOMBRE = crearEventoDTO.NOMBRE,
                FECHA_INICIO = crearEventoDTO.FECHA_INICIO,
                FECHA_FIN = crearEventoDTO.FECHA_FIN,
                NOMBRE_LUGAR = crearEventoDTO.NOMBRE_LUGAR,
                ID_ESTADO = crearEventoDTO.ID_ESTADO,
                ESTADO = estadoDTO,
                ID_UBIGEO = crearEventoDTO.ID_UBIGEO,
                DISTRITO = distritoDTO,
                BANNER = banerImagenFormato,
                FORMATO = banerImagenFormato,
                NUM_PARTICIPANTES = crearEventoDTO.NUM_PARTICIPANTES,
                DESCRIPCION = crearEventoDTO.DESCRIPCION,
                ENLACE_WHATSAPP = crearEventoDTO.ENLACE_WHATSAPP,
                HORA_INICIO = TimeOnly.Parse(crearEventoDTO.HORA_INICIO!),
                HORA_FIN = TimeOnly.Parse(crearEventoDTO.HORA_FIN!),
                RED_SOCIAL = crearEventoDTO.RED_SOCIAL,
                ID_TIPO_EVENTO = crearEventoDTO.ID_TIPO_EVENTO,
                TIPO_EVENTO = tipoeventoDTO,
                ID_MODALIDAD = crearEventoDTO.ID_MODALIDAD,
                MODALIDAD = modalidadDTO,
                GENERA_TICKET = crearEventoDTO.GENERA_TICKET,
                ENVIA_CORREO = crearEventoDTO.ENVIA_CORREO,
                REQUIERE_DOCUMENTO = crearEventoDTO.REQUIERE_DOCUMENTO,
                Documentos = eventoDocumentosDTO

            }; 

            eventoDTO.ID_EVENTO = id;     
             
            return TypedResults.Created($"/eventos/{id}", eventoDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(
              int id
            , CrearEventoDTO crearEventoDTO
            , IRepositorioEventos repositorio
            , IRepositorioEventosFechas repositorioEventosFechas
            , IRepositorioEventosDocumentos repositorioEventosDocumentos
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            , IWebHostEnvironment env
            , IConfiguration Configuration

            )
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
                return TypedResults.NotFound();

            if (!result.Any())
                return TypedResults.NotFound();

            var evento = new Evento
            {
                ID_EVENTO = id,
                NOMBRE = crearEventoDTO.NOMBRE,
                FECHA_INICIO = crearEventoDTO.FECHA_INICIO,
                FECHA_FIN = crearEventoDTO.FECHA_FIN,
                NOMBRE_LUGAR = crearEventoDTO.NOMBRE_LUGAR,
                ID_ESTADO = crearEventoDTO.ID_ESTADO,
                ID_UBIGEO = crearEventoDTO.ID_UBIGEO,
                BANNER = (crearEventoDTO.BANNER == "" ? result.FirstOrDefault().BANNER : crearEventoDTO.BANNER),
                FORMATO = (crearEventoDTO.FORMATO == "" ? result.FirstOrDefault().FORMATO : crearEventoDTO.FORMATO),
                NUM_PARTICIPANTES = crearEventoDTO.NUM_PARTICIPANTES,
                DESCRIPCION = crearEventoDTO.DESCRIPCION,
                ENLACE_WHATSAPP = crearEventoDTO.ENLACE_WHATSAPP,
                HORA_INICIO = TimeOnly.Parse(crearEventoDTO.HORA_INICIO!),
                HORA_FIN = TimeOnly.Parse(crearEventoDTO.HORA_FIN!),
                RED_SOCIAL = crearEventoDTO.RED_SOCIAL,
                ID_TIPO_EVENTO = crearEventoDTO.ID_TIPO_EVENTO,
                ID_MODALIDAD = crearEventoDTO.ID_MODALIDAD,
                GENERA_TICKET = crearEventoDTO.GENERA_TICKET,
                ENVIA_CORREO = crearEventoDTO.ENVIA_CORREO,
                ID_CUESTIONARIO = crearEventoDTO.ID_CUESTIONARIO,
                ID_ENCUESTA= crearEventoDTO.ID_ENCUESTA,
                REQUIERE_DOCUMENTO= crearEventoDTO.REQUIERE_DOCUMENTO,
                USER_UPD = "JFLORES"
            };


            var banerImagenFormato = Guid.NewGuid() + ".png";

            if (crearEventoDTO.BANNER is not null)
            {
                if (crearEventoDTO.BANNER.Trim() != "")
                { 
                    var carpetaEventos = Configuration.GetSection("folferEventos").Value;
                    var folderEventos = Path.Combine(env.WebRootPath, carpetaEventos);
                    if (!Directory.Exists(folderEventos))
                    {
                        Directory.CreateDirectory(folderEventos);
                    }

                    var folderEvento = Path.Combine(folderEventos, result.FirstOrDefault().COD_EVENTO);
                    if (!Directory.Exists(folderEvento))
                    {
                        Directory.CreateDirectory(folderEvento);
                    }

                    var folderEventoBanner = Path.Combine(folderEvento, "banner");
                    if (!Directory.Exists(folderEventoBanner))
                    {
                        Directory.CreateDirectory(folderEventoBanner);
                    }

                   
                    var banner = Path.Combine(folderEventoBanner, banerImagenFormato);
                    var bytess = Convert.FromBase64String(crearEventoDTO.BANNER);
                    using var imageFile = new FileStream(banner, FileMode.Create);
                    imageFile.Write(bytess, 0, bytess.Length);
                    imageFile.Flush();

                    evento.BANNER = banerImagenFormato;
                }
            }

            if (crearEventoDTO.FORMATO is not null)
            {
                if (crearEventoDTO.FORMATO.Trim() != "")
                {
                    var carpetaEventos = Configuration.GetSection("folferEventos").Value;
                    var folderEventos = Path.Combine(env.WebRootPath, carpetaEventos);
                    if (!Directory.Exists(folderEventos))
                    {
                        Directory.CreateDirectory(folderEventos);
                    }

                    var folderEvento = Path.Combine(folderEventos, result.FirstOrDefault().COD_EVENTO);
                    if (!Directory.Exists(folderEvento))
                    {
                        Directory.CreateDirectory(folderEvento);
                    }

                    var folderEventoBanner = Path.Combine(folderEvento, "formato");
                    if (!Directory.Exists(folderEventoBanner))
                    {
                        Directory.CreateDirectory(folderEventoBanner);
                    }

                    var banner = Path.Combine(folderEventoBanner, banerImagenFormato);
                    var bytess = Convert.FromBase64String(crearEventoDTO.FORMATO);
                    using var imageFile = new FileStream(banner, FileMode.Create);
                    imageFile.Write(bytess, 0, bytess.Length);
                    imageFile.Flush();

                    evento.FORMATO = banerImagenFormato;
                }
            }

            await repositorio.Actualizar(evento);

            if (crearEventoDTO.Documentos is not null)
            {
                await repositorioEventosDocumentos.Eliminar(id);

                foreach (var item in crearEventoDTO.Documentos)
                {
                    EventoDocumento eventoDocumento = new()
                    {
                        ID_EVENTO = id,
                        ID_TIPO_DOCUMENTO_REQUERIDO = item
                    };

                    await repositorioEventosDocumentos.Agregar(eventoDocumento);
                }              
            }

            var listFechas = await repositorioEventosFechas.Listar(id);
            foreach (var fecha in listFechas)
            {
                await repositorioEventosFechas.Eliminar(id, fecha.FECHA);
            }

            if (crearEventoDTO.FECHAS is not null)
            {
                foreach (CrearEventoFechaDTO item in crearEventoDTO.FECHAS)
                {
                    EventoFecha eventoFecha = new()
                    {
                        ES_CUESTIONARIO = item.ES_CUESTIONARIO,
                        ES_ENCUESTA = item.ES_ENCUESTA,
                        FECHA = item.FECHA,
                        HORA_FIN = item.HORA_FIN,
                        HORA_INICIO = item.HORA_INICIO,
                        ID_CUESTIONARIO = item.ID_CUESTIONARIO,
                        ID_ENCUESTA = item.ID_ENCUESTA,
                        ID_EVENTO = id
                    };

                    await repositorioEventosFechas.Agregar(eventoFecha);
                }
            }


            return TypedResults.NoContent();
        }


        static async Task<Results<NoContent, NotFound>> ActualizarFechas(
            int id
           , List<CrearEventoFechaDTO> crearEventoFechaDTO
           , IRepositorioEventos repositorioEventos
           , IRepositorioEventosFechas repositorioEventosFechas
           , IOutputCacheStore outputCacheStore
           , IMapper mapper
          )
        {

            var result = await repositorioEventos.Obtener(id);
            if (result is null)
                return TypedResults.NotFound();

            if (!result.Any())
                return TypedResults.NotFound();

            foreach (CrearEventoFechaDTO item in crearEventoFechaDTO)
            {

                var eventoFecha = mapper.Map<EventoFecha>(item);
                eventoFecha.ID_EVENTO = id;

                var existe = await repositorioEventosFechas.Obtener(id, item.FECHA);
                if (existe is not null)
                {
                    if (existe.Any())
                    {
                        await repositorioEventosFechas.Actualizar(eventoFecha);
                    }
                }
            }

            return TypedResults.NoContent();
        }



        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioEventos repositorio
            , IOutputCacheStore outputCacheStore)
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
                return TypedResults.NotFound();

            if (!result.Any())            
                return TypedResults.NotFound();

            await repositorio.Eliminar(id);
            return TypedResults.NoContent();
        }

    }
}
