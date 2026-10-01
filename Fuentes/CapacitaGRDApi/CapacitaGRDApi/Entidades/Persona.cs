using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class Persona : Registros
    {

        [Key]
        public int ID_PERSONA { get; set; }

        [StringLength(250)]
        public string COD_PERSONA { get; set; }

        public int ID_TIPO_DOCUMENTO { get; set; }

        [StringLength(50)]
        public string NUM_DOCUMENTO { get; set; }

        [StringLength(250)]
        public string NOMBRES { get; set; }

        [StringLength(250)]
        public string APELLIDO_PATERNO { get; set; }

        [StringLength(250)]
        public string APELLIDO_MATERNO { get; set; }

        [StringLength(1)]
        public string SEXO { get; set; }

        public int ID_PAIS_NACIMIENTO { get; set; }

        public DateTime? FECHA_NACIMIENTO { get; set; }

        public DateTime? FECHA_REG { get; set; }

        public DateTime? FECHA_UPD { get; set; }

        [StringLength(250)]
        public string EMAIL { get; set; }

        [StringLength(50)]
        public string CELULAR { get; set; } = null!;

        public int? ID_PROFESION { get; set; } = null!;

        [StringLength(1)]
        public string VALIDADO_PIDE  { get; set; } = null!;


         
    }
}
