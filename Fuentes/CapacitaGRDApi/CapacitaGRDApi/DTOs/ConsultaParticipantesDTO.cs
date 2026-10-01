namespace CapacitaGRDApi.DTOs
{
    public class ConsultaParticipantesDTO
    {
        public List<TipoDocumentDTO> TipoDocumento { get; set; } = null;
        public List<PaisDTO> Pais { get; set; } = null;
        public List<ProfesionDTO> Profesion { get; set; } = null;

        public List<DistritoDTO> Distrito { get; set; } = null;

        public List<CargoDTO> Cargo { get; set; } = null;

        public List<EntidadDTO> Entidad { get; set; } = null;

        public List<EventoDTO> Evento { get; set; } = null;

    }
}
