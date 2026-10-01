
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.DTOs
{
    public class BusquedaEventoDTO
    {
        public FilterDataTable Filtro { get; set; }

        public int? Modalidad { get; set; }

        public int? Tipo { get; set; }
        public int? Estado{ get; set; }


    }
}
