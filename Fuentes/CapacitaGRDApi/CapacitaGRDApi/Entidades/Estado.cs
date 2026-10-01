using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Estado : Registros
    {
        [Key]
        public int ID_ESTADO { get; set; }

        [StringLength(250)]
        public String NOMBRE { get; set; } = null!;

    }
}
