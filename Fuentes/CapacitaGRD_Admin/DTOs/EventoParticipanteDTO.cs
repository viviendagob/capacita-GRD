namespace CapacitaGRD_Admin.DTOs
{
    public class EventoParticipanteDTO
    {
        public int ID_EVENTO { get; set; }
        public int ID_PERSONA_DATA { get; set; }
        public int ID_MODALIDAD { get; set; }
        public DateTime FECHA_REG { get; set; }
        public Double? LATITUD { get; set; }
        public Double? LONGITUD { get; set; }
        public int ID_PERSONA { get; set; }
        public EventoDTO Evento { get; set; }

    }
}
