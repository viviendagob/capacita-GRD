using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Cuestionario : Registros
    {
 
        [Key]
        public int ID_CUESTIONARIO { get; set; }

        [StringLength(250)]
        public string NOMBRE { get; set; } = null!;
 

        public string USER_REG { get; set; } = null!;

        public DateTime? FECHA_REG { get; set; }

        public string? USER_UPD { get; set; } = null!;

        public DateTime? FECHA_UPD { get; set; }
    }
}
