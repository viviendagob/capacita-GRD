using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class TipoEvento : Registros
    {
        [Key]
        public int ID_TIPO_EVENTO { get; set; }

        [StringLength(250)]
        public String NOMBRE { get; set; } = null!;

    }
}
