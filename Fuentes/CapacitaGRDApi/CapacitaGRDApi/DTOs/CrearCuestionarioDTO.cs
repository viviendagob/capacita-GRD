namespace CapacitaGRDApi.DTOs
{
    public class CrearCuestionarioDTO
    {

        public String NOMBRE { get; set; } = null!;

        public List<CrearCuestionarioPreguntaDTO>? PREGUNTAS { get; set; }

    }
}
