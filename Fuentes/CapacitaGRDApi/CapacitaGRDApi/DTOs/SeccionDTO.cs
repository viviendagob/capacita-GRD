using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.DTOs
{
    public class SeccionDTO
    {
        public int ID_SECCION { get; set; }

        public String NOMBRE { get; set; } = null!;
    }
}
