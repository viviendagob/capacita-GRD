namespace CapacitaGRDApi.DTOs
{
    public class ReporteAsistenciaDTO
    {
        public int ID_PERSONA { get; set; }
        public string TIPO_DOCUMENTO { get; set; } = null!;
        public string NUM_DOCUMENTO { get; set; } = null!;
        public string NOMBRES { get; set; } = null!;
        public string APELLIDO_PATERNO { get; set; } = null!;
        public string APELLIDO_MATERNO { get; set; } = null!;
        public string MODALIDAD { get; set; } = null!;
        public DateTime FECHA_REG { get; set; }
    }
}
