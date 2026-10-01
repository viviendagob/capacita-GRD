using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Entidad : Registros
    {
        [Key]
        public int ID_ENTIDAD { get; set; }

        [StringLength(250)]
        public String NOMBRE { get; set; } = null!;

    }
}
