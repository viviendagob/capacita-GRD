
using static CapacitaGRD_Admin.Entidades.Paginado;

namespace CapacitaGRDApi.DTOs
{
    public class BusquedaGenericaDTO
    {
        public FilterDataTable Filtro { get; set; }

        public String Dato { get; set; } = null!;
    }
}
