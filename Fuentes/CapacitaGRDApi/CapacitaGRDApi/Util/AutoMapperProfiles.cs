using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.EndPoints;
using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Util
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearCargoDTO, Cargo>();
            CreateMap<Cargo, CargoDTO>();                       

            CreateMap<CrearProfesionDTO, Profesion>();
            CreateMap<Profesion, ProfesionDTO>();

            CreateMap<CrearPaisDTO, Pais>();
            CreateMap<Pais, PaisDTO>();

            CreateMap<CrearUsuarioDTO, Usuarios>();
            CreateMap<Usuarios, UsuarioDTO>();

            CreateMap<CrearDistritoDTO, Distrito>();
            CreateMap<Distrito, DistritoDTO>();

            CreateMap<CrearEventoDTO, Evento>();
            CreateMap<Evento, EventoDTO>();

            CreateMap<CrearEncuestaDTO, Encuesta>();
            CreateMap<Encuesta, EncuestaDTO>();

            CreateMap<CrearCuestionarioDTO, Cuestionario>();
            CreateMap<Cuestionario, CuestionarioDTO>();

            CreateMap<CrearCuestionarioPreguntaDTO, CuestionarioPregunta>();
            CreateMap<CuestionarioPregunta, CuestionarioPreguntaDTO>();

            CreateMap<CrearCuestionarioPreguntaRespuestaDTO, CuestionarioPreguntaRespuesta>();
            CreateMap<CuestionarioPreguntaRespuesta, CuestionarioPreguntaRespuestaDTO>();

            CreateMap<CrearEventoFechaDTO, EventoFecha>();
            CreateMap<EventoFecha, EventoFechaDTO>();

            CreateMap<CrearEncuestaRespuestaDTO, EncuestaRespuesta>();
            CreateMap<EncuestaRespuesta, EncuestaRespuestaDTO>();

            CreateMap<CrearPersonaDTO, Persona>();
            CreateMap<Persona, PersonaDTO>();

            CreateMap<CrearPersonaDataDTO, PersonaData>();
            CreateMap<PersonaData, PersonaDataDTO>();

            CreateMap<CrearEventoParticipanteDTO, EventoParticipante>();
            CreateMap<EventoParticipante, EventoParticipanteDTO>();

            CreateMap<CrearEventoAsistenciaDTO, EventoAsistencia>();
            CreateMap<EventoAsistencia, EventoAsistenciaDTO>();

            CreateMap<CrearSeccionDTO, Seccion>();
            CreateMap<Seccion, SeccionDTO>();

            CreateMap<CrearDocumentoDTO, Documento>();
            CreateMap<Documento, DocumentoDTO>();
            
            CreateMap<EventoDocumento, EventoDocumentoDTO>();

            CreateMap<CrearConfiguracionDTO, Configuracion>();
            CreateMap<Configuracion, ConfiguracionDTO>();
            
            CreateMap<Entidad, EntidadDTO>();
            CreateMap<TipoDocumento, TipoDocumentDTO>();
            CreateMap<Estado, EstadoDTO>();

            CreateMap<TipoEvento, TipoEventoDTO>();
 
            CreateMap<Modalidad, ModalidadDTO>();

            CreateMap<TipoEncuestaRespuesta, TipoEncuestaRespuestaDTO>();
        }
    }
}
