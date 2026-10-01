namespace CapacitaGRDApi.DTOs
{
    public class CuestionarioPreguntaRespuestaDTO
    {
        public int ID_PREGUNTA_RESPUESTA { get; set; }

        public string NOMBRE { get; set; } = null!;

        public int ID_CUESTIONARIO_PREGUNTA { get; set; }
    }
}
