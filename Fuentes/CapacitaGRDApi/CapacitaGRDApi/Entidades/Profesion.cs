using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Profesion : Registros
    {
        [Key]
        public int ID_PROFESION { get; set; }

        [StringLength(250)]
        public String NOMBRE { get; set; } = null!;

    }
}
