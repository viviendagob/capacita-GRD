using System.ComponentModel.DataAnnotations;

namespace CapacitaGRD_Admin.DTOs
{
    public class EncuestaRespuestaDTO
    {
        public int ID_RESPUESTA { get; set; }

        public string NOMBRE { get; set; } = null!;

        public string? RESPUESTAS { get; set; } = null!;

        public int ID_ENCUESTA { get; set; }

        public int ID_TIPO_ENCUESTA_PREGUNTA { get; set; }

        public TipoEncuestaRespuestaDTO TIPO_ENCUESTA_PREGUNTA { get; set; }
}
}
