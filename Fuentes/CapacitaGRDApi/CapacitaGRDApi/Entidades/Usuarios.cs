using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Usuarios : Registros
    {
        [Key]
        public int ID_USUARIO { get; set; }

        [StringLength(100) ]
        public String USUARIO { get; set; } = null!;

        [StringLength(100)]
        public String CLAVE { get; set; } = null!;

        public int ESTADO { get; set; }

    }
}
