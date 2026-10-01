namespace CapacitaGRD_Admin.DTOs
{
    public class CrearEncuestaDTO
    {

        public String NOMBRE { get; set; } = null!;

        public List<CrearEncuestaRespuestaDTO>? RESPUESTAS { get; set; }

    }
}
