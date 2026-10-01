using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.DTOs
{
    public class PaginadorDTO<T>
    {
        public int? draw { get; set; }
        public Int64? recordsFiltered { get; set; }
        public Int64? recordsTotal { get; set; }
        public List<T> data {get; set;}
    }

}