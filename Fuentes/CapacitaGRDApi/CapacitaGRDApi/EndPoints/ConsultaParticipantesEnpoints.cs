using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class ConsultaParticipantesEnpoints
    {
        public static RouteGroupBuilder ConsultaParticipantes(this RouteGroupBuilder group)
        {
            group.MapGet("/maestros", Maestros);
            group.MapPost("/paginado", Paginar);

            return group;
        }
        static async Task<Ok<ConsultaParticipantesDTO>> Maestros(
            IRepositorioTipoDocumentos repositorioTipoDocumentos
            , IRepositorioPais repositorioPais
            , IRepositorioProfesiones repositorioProfesiones
            , IRepositorioCargos repositorioCargos
            , IRepositorioDistritos repositorioDistritos
            , IRepositorioEntidades repositorioEntidades
            , IRepositorioEventos repositorioEventos
            , IMapper mapper)
        {
            var tipoDocumentos = await repositorioTipoDocumentos.Listar();
            var tipoDocumentosDTO = mapper.Map<List<TipoDocumentDTO>>(tipoDocumentos);

            var pais = await repositorioPais.Listar();
            var paisDTO = mapper.Map<List<PaisDTO>>(pais);

            var profesiones = await repositorioProfesiones.Listar();
            var profesionesDTO = mapper.Map<List<ProfesionDTO>>(profesiones);

            var distritos = await repositorioDistritos.Listar();
            var distritosDTO = mapper.Map<List<DistritoDTO>>(distritos);

            var cargos = await repositorioCargos.Listar();
            var cargosDTO = mapper.Map<List<CargoDTO>>(cargos);

            var entidades  = await repositorioEntidades.Listar();
            var entidadesDTO = mapper.Map<List<EntidadDTO>>(entidades);

            var eventos = await repositorioEventos.Listar();
            var eventosDTO = mapper.Map<List<EventoDTO>>(eventos);


            var consultaParticipantesDTO = new ConsultaParticipantesDTO
            {
                TipoDocumento = tipoDocumentosDTO,
                Pais = paisDTO,
                Profesion = profesionesDTO,
                Distrito = distritosDTO, 
                Cargo = cargosDTO,
                Entidad = entidadesDTO,
                Evento = eventosDTO
            };

            return TypedResults.Ok(consultaParticipantesDTO);
        }

        static async Task<Ok<PaginadorDTO<PersonaDTO>>> Paginar(
      BusquedaConsultaParticipanteDTO filtro
    , IRepositorioPersonas repositorio
    , IRepositorioTipoDocumentos repositorioTipoDocumentos
    , IRepositorioEventosAsistencias repositorioEventosAsistencias
    , IMapper mapper)
        {
            var paginado = filtro.Filtro;
            var tipoDocumento = filtro.TipoDocumento;
            var numeroDocumento = filtro.NumeroDocumento;
            var paterno = filtro.Paterno;
            var materno = filtro.Materno;
            var idEvento = filtro.idEvento;


            int pagActual = paginado.start;
            int pagDesde = (pagActual == 0) ? 1 : (pagActual + 1);
            int pagHasta = pagActual + paginado.length;
            var inxColumn = paginado.order[0].column;
            var orderColumn = paginado.order[0].dir;
            var orderBy = paginado.columns[inxColumn].name + ' ' + orderColumn;

            var where = " ";
            if (tipoDocumento != 0)
            {
                where += " AND ID_TIPO_DOCUMENTO = " + tipoDocumento;
            }

            if (numeroDocumento.Trim() != "")
            {
                where += " AND NUM_DOCUMENTO LIKE '%" + numeroDocumento.Replace("'", "''") + "%' ";
            }

            if (paterno.Trim() != "")
            {
                where += " AND APELLIDO_PATERNO LIKE '%" + paterno.Replace("'", "''") + "%' ";
            }

            if (materno.Trim() != "")
            {
                where += " AND APELLIDO_MATERNO LIKE '%" + materno.Replace("'", "''") + "%' ";
            }
             
            if (idEvento.HasValue && idEvento.Value != 0)
            {
                where += " AND ID_PERSONA IN (SELECT EP.ID_PERSONA from dbo.EVENTO_PARTICIPANTE EP WHERE EP.ID_EVENTO = " + idEvento.Value + ") ";
            }


            Filter filter = new()
            {
                Limit = "  FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var personas = await repositorio.Paginar(filter);
            var personasDTO = mapper.Map<List<PersonaDTO>>(personas);

            foreach (var personaDTO in personasDTO)
            {
                var resultTipoDocumento = await repositorioTipoDocumentos.Obtener(personaDTO.ID_TIPO_DOCUMENTO);
                var _tipoDocumento = resultTipoDocumento.FirstOrDefault();
                var tipoDocumentDTO = mapper.Map<TipoDocumentDTO>(_tipoDocumento);
                personaDTO.TIPO_DOCUMENTO = tipoDocumentDTO;

                if (idEvento.HasValue && idEvento.Value != 0)
                {
                    var asistencias = await repositorioEventosAsistencias.Listar(idEvento.Value, personaDTO.ID_PERSONA);
                    personaDTO.Asistencias = mapper.Map<List<EventoAsistenciaDTO>>(asistencias);
                }
            }

            var response = new PaginadorDTO<PersonaDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (personas.Any() ? personas.FirstOrDefault().REGISTROS : 0),
                recordsTotal = (personas.Any() ? personas.FirstOrDefault().REGISTROS : 0),
                data = personasDTO
            };

            return TypedResults.Ok(response);
        }


    }
}
