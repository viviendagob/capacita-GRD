using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.DTOs
{
    public class CargoDTO
    {
        public int ID_CARGO { get; set; }

        public String NOMBRE { get; set; } = null!;
    }
}
