using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class TipoDocumento : Registros
    {
        [Key]
        public int ID_TIPO_DOCUMENTO { get; set; }

        [StringLength(250)]
        public String NOMBRE { get; set; } = null!;

    }
}
