namespace CapacitaGRD_Admin.DTOs
{
    public class ParticipanteConstanciaDTO
    {
        public int ID_PERSONA { get; set; }
        public string NUM_DOCUMENTO { get; set; } = null!;
        public string NOMBRES { get; set; } = null!;
        public string APELLIDO_PATERNO { get; set; } = null!;
        public string APELLIDO_MATERNO { get; set; } = null!;
        public int SESIONES_OBLIGATORIAS { get; set; }
        public int SESIONES_ASISTIDAS { get; set; }
        public bool ENCUESTA_RESPONDIDA { get; set; }
        public bool APTO { get; set; }
        public ConstanciaDTO? Constancia { get; set; }
    }

    public class ConstanciaDTO
    {
        public int ID_CONSTANCIA { get; set; }
        public int ID_EVENTO { get; set; }
        public int ID_PERSONA { get; set; }
        public string CODIGO { get; set; } = null!;
        public string ESTADO { get; set; } = null!;
        public string? URL_PDF { get; set; }
        public DateTime FECHA_GENERACION { get; set; }
        public string? USER_GENERACION { get; set; }
        public DateTime? FECHA_ULTIMA_DESCARGA { get; set; }
        public int NUM_DESCARGAS { get; set; }
        public string? USER_ANULACION { get; set; }
        public DateTime? FECHA_ANULACION { get; set; }
        public string? MOTIVO_ANULACION { get; set; }
    }

    public class AnularConstanciaDTO
    {
        public string MOTIVO { get; set; } = null!;
    }
}
