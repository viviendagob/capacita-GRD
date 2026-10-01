
namespace CapacitaGRD_Admin.Entidades
{
    public class Encuesta 
    {
 
        public int ID_ENCUESTA { get; set; }

        public string NOMBRE { get; set; } = null!;
 

        public string USER_REG { get; set; } = null!;

        public DateTime? FECHA_REG { get; set; }

        public string? USER_UPD { get; set; } = null!;

        public DateTime? FECHA_UPD { get; set; }
    }
}
