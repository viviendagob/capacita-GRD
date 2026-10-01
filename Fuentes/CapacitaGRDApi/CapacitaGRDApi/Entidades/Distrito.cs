using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Distrito  : Registros {

        [Key]
        public int ID_DISTRITO { get; set; }

        [StringLength(2)]
        public string CODIGO_DEPARTAMENTO { get; set; } = null!;

        [StringLength(250)]
        public string DEPARTAMENTO { get; set; } = null!;

        [StringLength(4)]
        public string CODIGO_PROVINCIA { get; set; } = null!;

        [StringLength(250)]
        public string PROVINCIA { get; set; } = null!;

        [StringLength(6)]
        public string CODIGO_DISTRITO { get; set; } = null!;

        [StringLength(250)]
        public string DISTRITO { get; set; } = null!;

        [StringLength(250)]
        public string CAPITAL { get; set; } = null!;

        [StringLength(1)]
        public string CATEGORIA { get; set; } = null!;

        public int ID_PAIS { get; set; }

    }
}
