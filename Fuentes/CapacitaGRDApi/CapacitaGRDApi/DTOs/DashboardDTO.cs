namespace CapacitaGRDApi.DTOs
{
    public class DashboardFiltroDTO
    {
        public int? ID_EVENTO { get; set; }
        public DateTime? FECHA_DESDE { get; set; }
        public DateTime? FECHA_HASTA { get; set; }
        public int? ID_MODALIDAD { get; set; }
        public string? DEPARTAMENTO { get; set; }
        public string? PROVINCIA { get; set; }
        public string? DISTRITO { get; set; }
        public int? ID_ENTIDAD { get; set; }
        public int? ID_ESTADO { get; set; }
    }

    public class KpiSerieDTO
    {
        public string ETIQUETA { get; set; } = null!;
        public int CANTIDAD { get; set; }
    }

    public class DashboardDTO
    {
        public int TOTAL_EVENTOS { get; set; }
        public int TOTAL_PARTICIPANTES { get; set; }
        public int TOTAL_ASISTENTES { get; set; }
        public double PCT_ASISTENCIA { get; set; }
        public int TOTAL_CONSTANCIAS_GENERADAS { get; set; }
        public int TOTAL_CONSTANCIAS_ANULADAS { get; set; }
        public double PCT_APTO_CONSTANCIA { get; set; }
        public int TOTAL_RESPONDIERON_ENCUESTA { get; set; }
        public double PCT_RESPUESTA_ENCUESTA { get; set; }
        public double PCT_SATISFACCION { get; set; }
        public int TOTAL_INSTITUCIONES { get; set; }
        public int TOTAL_DISTRITOS { get; set; }
        public List<KpiSerieDTO> POR_MODALIDAD { get; set; } = new();
        public List<KpiSerieDTO> POR_DEPARTAMENTO { get; set; } = new();
        public List<KpiSerieDTO> POR_ESTADO_EVENTO { get; set; } = new();
    }
}
