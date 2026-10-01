namespace CapacitaGRDApi.DTOs
{
    public class CuestionarioPreguntaDTO
    {
        public int ID_CUESTIONARIO_PREGUNTA { get; set; }

        public string NOMBRE { get; set; } = null!;

        public int PESO { get; set; }

        public int ID_CUESTIONARIO { get; set; }

        public List<CuestionarioPreguntaRespuestaDTO>? RESPUESTAS { get; set; }
    }
}
