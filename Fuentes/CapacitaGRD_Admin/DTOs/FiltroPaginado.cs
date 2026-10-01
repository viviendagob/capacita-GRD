using static CapacitaGRD_Admin.Entidades.Paginado;

namespace CapacitaGRD_Admin.DTOs
{
    public class FiltroPaginado
    {
        public FilterDataTable filtro { get; set; }

        public string buscar { get; set; } = null!;

    }
}
