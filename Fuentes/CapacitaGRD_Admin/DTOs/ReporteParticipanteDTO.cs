namespace CapacitaGRD_Admin.DTOs
{
    public class ReporteParticipanteDTO
    {
        public int ID_PERSONA { get; set; }
        public string NUM_DOCUMENTO { get; set; } = null!;
        public string NOMBRES { get; set; } = null!;
        public string APELLIDO_PATERNO { get; set; } = null!;
        public string APELLIDO_MATERNO { get; set; } = null!;
        public string? EMAIL { get; set; }
        public string? CELULAR { get; set; }
        public string? DEPARTAMENTO { get; set; }
        public string? PROVINCIA { get; set; }
        public string? DISTRITO { get; set; }
        public string? ENTIDAD { get; set; }
        public string? CARGO { get; set; }
        public string? MODALIDAD { get; set; }
        public int SESIONES_TOTALES { get; set; }
        public int SESIONES_OBLIGATORIAS { get; set; }
        public int SESIONES_ASISTIDAS { get; set; }
        public bool ENCUESTA_RESPONDIDA { get; set; }
        public bool APTO_CONSTANCIA { get; set; }
        public string ESTADO_CONSTANCIA { get; set; } = null!;
        public string? CODIGO_CONSTANCIA { get; set; }
        public DateTime FECHA_INSCRIPCION { get; set; }
    }
}
