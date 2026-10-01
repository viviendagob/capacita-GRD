using System.ComponentModel.DataAnnotations;

namespace CapacitaGRD_Admin.DTOs
{
    public class CrearEncuestaRespuestaDTO
    {
        public string NOMBRE { get; set; } = null!;

        public string? RESPUESTAS { get; set; } = null!;

        public int ID_ENCUESTA { get; set; }

        public int ID_TIPO_ENCUESTA_PREGUNTA { get; set; }

    }
}
