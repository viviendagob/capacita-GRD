using static CapacitaGRD_Admin.Entidades.Paginado;

namespace CapacitaGRD_Admin.DTOs
{
    public class BusquedaConsultaParticipanteDTO
    {
        public FilterDataTable Filtro { get; set; }

        public int? idEvento { get; set; }

        public int? TipoDocumento { get; set; }

        public String NumeroDocumento { get; set; } = null!;

        public String Paterno { get; set; } = null!;

        public String Materno { get; set; } = null!;

    }
}
