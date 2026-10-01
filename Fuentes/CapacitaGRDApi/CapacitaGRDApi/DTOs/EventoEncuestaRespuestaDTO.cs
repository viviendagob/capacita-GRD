namespace CapacitaGRDApi.DTOs
{
    public class CrearEventoEncuestaRespuestaDTO
    {
        public int ID_EVENTO { get; set; }
        public int ID_PERSONA { get; set; }
        public int ID_ENCUESTA { get; set; }
        public int ID_PREGUNTA { get; set; }
        public string RESPUESTA { get; set; } = null!;
    }

    public class EstadisticaEncuestaDTO
    {
        public int TOTAL_ASISTENTES { get; set; }
        public int TOTAL_RESPONDIERON { get; set; }
        public double PORCENTAJE_RESPUESTA { get; set; }
        public double META_RESPUESTA { get; set; } = 60;
        public double PORCENTAJE_SATISFECHOS { get; set; }
        public List<EstadisticaPreguntaDTO> Preguntas { get; set; } = new();
    }

    public class EstadisticaPreguntaDTO
    {
        public int ID_PREGUNTA { get; set; }
        public string PREGUNTA { get; set; } = null!;
        public List<EstadisticaAlternativaDTO> Alternativas { get; set; } = new();
    }

    public class EstadisticaAlternativaDTO
    {
        public string RESPUESTA { get; set; } = null!;
        public int CANTIDAD { get; set; }
    }
}
