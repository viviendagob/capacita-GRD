using System.ComponentModel.DataAnnotations;

namespace CapacitaGRD_Admin.DTOs
{
    public class LoginDTO
    {
        public String username { get; set; }

        public String password { get; set; } // = null!;

        public String Token { get; set; }
    }
}
