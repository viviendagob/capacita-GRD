using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.DTOs
{
    public class TipoEncuestaRespuestaDTO
    {
        public int ID_TIPO_ENCUESTA_PREGUNTA { get; set; }

        public String NOMBRE { get; set; } = null!;
    }
}
