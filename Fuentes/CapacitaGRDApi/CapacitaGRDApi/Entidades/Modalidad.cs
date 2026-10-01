using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Modalidad : Registros
    {
        [Key]
        public int ID_MODALIDAD { get; set; }

        [StringLength(250)]
        public String NOMBRE { get; set; } = null!;

    }
}
