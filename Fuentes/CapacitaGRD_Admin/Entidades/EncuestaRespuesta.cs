
namespace CapacitaGRD_Admin.Entidades
{
    public class EncuestaRespuesta 
    {
 
        public int ID_RESPUESTA { get; set; }

        public string? NOMBRE { get; set; } = null!;

        public string RESPUESTAS { get; set; } = null!;

        public int ID_ENCUESTA { get; set; }

        public int ID_TIPO_ENCUESTA_PREGUNTA { get; set; }

        
         
    }
}
