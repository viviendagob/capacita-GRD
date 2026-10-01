using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Cargo : Registros
    {

        [Key]
        public int ID_CARGO { get; set; }

        [StringLength(250) ]
        public String NOMBRE { get; set; } = null!;

        
    }
}
