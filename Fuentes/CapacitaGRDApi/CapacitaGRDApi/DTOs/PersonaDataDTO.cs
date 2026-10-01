using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.DTOs
{
    public class PersonaDataDTO
    {
        public int ID_PERSONA_DATA { get; set; }

        public int ID_PERSONA { get; set; }

        public int ID_PAIS_LABORA { get; set; }

        public int ID_ENTIDAD { get; set; }

        public string NOMBRE_ENTIDAD_OTRA { get; set; } = null!;

        public string AREA_LABORA { get; set; } = null!;

        public int ID_CARGO { get; set; }

        public string NOMBRE_CARGO_OTRA { get; set; } = null!;

        public string ID_DISTRITO { get; set; } = null!;

        public DateTime? FECHA_REG { get; set; }


    }
}
