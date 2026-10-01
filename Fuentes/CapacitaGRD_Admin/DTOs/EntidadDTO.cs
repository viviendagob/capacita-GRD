using System.ComponentModel.DataAnnotations;

namespace CapacitaGRD_Admin.DTOs
{
    public class EntidadDTO
    {
        public int ID_ENTIDAD { get; set; }

        public String NOMBRE { get; set; } = null!;
    }
}
