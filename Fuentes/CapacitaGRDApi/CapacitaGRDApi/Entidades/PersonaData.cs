using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class PersonaData 
    {

        [Key]
        public int ID_PERSONA_DATA { get; set; }

        public int ID_PERSONA { get; set; }

        public int ID_PAIS_LABORA { get; set; }

        public int ID_ENTIDAD { get; set; }

        [StringLength(250)]
        public string NOMBRE_ENTIDAD_OTRA { get; set; } = null!;

        [StringLength(250)]
        public string AREA_LABORA { get; set; } = null!;

        public int ID_CARGO { get; set; }
         
        [StringLength(250)]
        public string NOMBRE_CARGO_OTRA { get; set; } = null!;

        public int ID_DISTRITO { get; set; }
 
        public DateTime? FECHA_REG { get; set; }

        
         
    }
}
