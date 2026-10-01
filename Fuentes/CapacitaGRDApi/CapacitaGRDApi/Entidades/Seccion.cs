using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Seccion : Registros
    {
        [Key]
        public int ID_SECCION { get; set; }

        [StringLength(250) ]
        public String NOMBRE { get; set; } = null!;
    }
}
