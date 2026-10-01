using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Documento : Registros
    {
 
        [Key]
        public int ID_TIPO_DOCUMENTO_REQUERIDO { get; set; }

        [StringLength(250)]
        public string NOMBRE { get; set; } = null!;
         
    }
}
