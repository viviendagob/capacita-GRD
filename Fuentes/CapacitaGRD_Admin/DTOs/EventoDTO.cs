 
namespace CapacitaGRD_Admin.DTOs
{
    public class EventoDTO
    {
        public int ID_EVENTO { get; set; }

        public string COD_EVENTO { get; set; } = null!;

        public string NOMBRE { get; set; } = null!;

        public DateTime FECHA_INICIO { get; set; }

        public DateTime FECHA_FIN { get; set; }

        public string NOMBRE_LUGAR { get; set; } = null!;

        public int ID_ESTADO { get; set; }
        public EstadoDTO? ESTADO { get; set; }

        public int ID_UBIGEO { get; set; }
        public DistritoDTO? DISTRITO { get; set; }

        public string BANNER { get; set; } = null!;

        public int NUM_PARTICIPANTES { get; set; }

        public string DESCRIPCION { get; set; } = null!;

        public string ENLACE_WHATSAPP { get; set; } = null!;

        public TimeOnly HORA_INICIO { get; set; }

        public TimeOnly HORA_FIN { get; set; }

        public string RED_SOCIAL { get; set; } = null!;

        public int ID_TIPO_EVENTO { get; set; }
        public TipoEventoDTO? TIPO_EVENTO { get; set; }

        public int ID_MODALIDAD { get; set; }
        public ModalidadDTO? MODALIDAD { get; set; }

        public string USER_REG { get; set; } = null!;

        public DateTime? FECHA_REG { get; set; }

        public string? USER_UPD { get; set; } = null!;

        public DateTime? FECHA_UPD { get; set; }

        public List<EventoFechaDTO>? FECHAS { get; set; } = null;
        
        public string GENERA_TICKET { get; set; } = null!;
        
        public string ENVIA_CORREO { get; set; } = null!;

        public int? ID_ENCUESTA { get; set; }
        public int? ID_CUESTIONARIO { get; set; }

        public string FORMATO { get; set; } = null!;

        public string REQUIERE_DOCUMENTO { get; set; } = null!;

        public List<EventoDocumentoDTO>? Documentos { get; set; } = null;
    }
}
