using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class EncuestaRespuesta : Registros
    {
 
        [Key]
        public int ID_RESPUESTA { get; set; }

        [StringLength(250)]
        public string? NOMBRE { get; set; } = null!;

        [StringLength(250)]
        public string RESPUESTAS { get; set; } = null!;

        public int ID_ENCUESTA { get; set; }

        public int ID_TIPO_ENCUESTA_PREGUNTA { get; set; }

        
         
    }
}
