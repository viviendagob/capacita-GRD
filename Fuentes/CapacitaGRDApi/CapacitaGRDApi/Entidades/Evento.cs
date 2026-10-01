using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Evento : Registros
    {
 
        [Key]
        public int ID_EVENTO { get; set; }

        [StringLength(250)]
        public string COD_EVENTO { get; set; } = null!;

        [StringLength(250)]
        public string NOMBRE { get; set; } = null!;

        public DateTime FECHA_INICIO { get; set; } 
        
        public DateTime FECHA_FIN { get; set; }

        [StringLength(250)]
        public string NOMBRE_LUGAR { get; set; } = null!;

        public int ID_ESTADO { get; set; }

        public int ID_UBIGEO { get; set; } 

        [StringLength(250)]
        public string BANNER { get; set; } = null!;

        public int NUM_PARTICIPANTES { get; set; }

        [StringLength(250)]
        public string DESCRIPCION { get; set; } = null!;

        [StringLength(250)]
        public string ENLACE_WHATSAPP { get; set; } = null!;

        public TimeOnly HORA_INICIO { get; set; }

        public TimeOnly HORA_FIN { get; set; }

        [StringLength(250)]
        public string RED_SOCIAL { get; set; } = null!;
        
        public int ID_TIPO_EVENTO { get; set; } 
        
        public int  ID_MODALIDAD { get; set; }

        public string USER_REG { get; set; } = null!;

        public DateTime? FECHA_REG { get; set; }

        public string? USER_UPD { get; set; } = null!;

        public DateTime? FECHA_UPD { get; set; }

        [StringLength(1)]
        public string GENERA_TICKET { get; set; } = null!;

        [StringLength(1)]
        public string ENVIA_CORREO { get; set; } = null!;

        public int? ID_ENCUESTA { get; set; }
        public int? ID_CUESTIONARIO { get; set; }

        public string FORMATO { get; set; } = null!;

        [StringLength(1)]
        public string REQUIERE_DOCUMENTO { get; set; } = null!;

    }
}
