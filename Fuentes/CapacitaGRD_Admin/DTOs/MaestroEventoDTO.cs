namespace CapacitaGRD_Admin.DTOs
{
    public class MaestroEventoDTO
    {
        public List<TipoEventoDTO> TipoEvento { get; set; } = null;
        public List<ModalidadDTO> ModalidadEvento { get; set; } = null;
        public List<EstadoDTO> EstadoEvento { get; set; } = null;
        public List<EncuestaDTO> Encuestas { get; set; } = null;
        public List<CuestionarioDTO> Cuestionarios { get; set; } = null;
        public List<DistritoDTO> Distritos { get; set; } = null;
        public List<DocumentoDTO> Documentos { get; set; } = null;

    }
}
