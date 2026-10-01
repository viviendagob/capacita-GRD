using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.DTOs
{
    public class EntidadDTO
    {
        public int ID_ENTIDAD { get; set; }

        public String NOMBRE { get; set; } = null!;
    }
}
