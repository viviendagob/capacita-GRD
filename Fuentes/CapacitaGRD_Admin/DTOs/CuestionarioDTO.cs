namespace CapacitaGRD_Admin.DTOs
{
    public class CuestionarioDTO
    {
        public int ID_CUESTIONARIO { get; set; }

        public string NOMBRE { get; set; } = null!; 

        public string USER_REG { get; set; } = null!;

        public DateTime? FECHA_REG { get; set; }

        public string? USER_UPD { get; set; } = null!;

        public DateTime? FECHA_UPD { get; set; }

        public List<CuestionarioPreguntaDTO>? PREGUNTAS { get; set; }

    }
}
