
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.DTOs
{
    public class BusquedaGenericaDTO
    {
        public FilterDataTable Filtro { get; set; }

        public String Dato { get; set; } = null!;
    }
}
