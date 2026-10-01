
using static CapacitaGRD_Admin.Entidades.Paginado;

namespace CapacitaGRDApi.DTOs
{
    public class BusquedaDistritoDTO
    {
        public FilterDataTable Filtro { get; set; }

        public int? IdPais { get; set; }

        public String Dato { get; set; } = null!;
    }
}
