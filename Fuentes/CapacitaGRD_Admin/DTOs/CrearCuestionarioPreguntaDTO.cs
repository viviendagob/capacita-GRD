namespace CapacitaGRD_Admin.DTOs
{
    public class CrearCuestionarioPreguntaDTO
    {
        public string NOMBRE { get; set; } = null!;

        public int PESO { get; set; }

        public List<CrearCuestionarioPreguntaRespuestaDTO>? RESPUESTAS { get; set; }
    }
}
