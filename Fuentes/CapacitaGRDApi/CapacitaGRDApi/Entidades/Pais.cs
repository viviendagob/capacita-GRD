using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Pais : Registros
    {
        [Key]
        public int ID_PAIS { get; set; }

        [StringLength(250)]
        public String NOMBRE { get; set; } = null!;

        [StringLength(1)]
        public String ESLOCAL { get; set; } = null!;

        [StringLength(50)]
        public String COD_PAIS { get; set; } = null!;
    }
}
