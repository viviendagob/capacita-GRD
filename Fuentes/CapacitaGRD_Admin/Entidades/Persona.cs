 
namespace CapacitaGRD_Admin.Entidades
{
    public class Persona  
    {

        public int ID_PERSONA { get; set; }

        public string COD_PERSONA { get; set; }

        public int ID_TIPO_DOCUMENTO { get; set; }

        public string NUM_DOCUMENTO { get; set; }

        public string NOMBRES { get; set; }

        public string APELLIDO_PATERNO { get; set; }

        public string APELLIDO_MATERNO { get; set; }

        public string SEXO { get; set; }

        public int ID_PAIS_NACIMIENTO { get; set; }

        public DateTime? FECHA_NACIMIENTO { get; set; }

        public DateTime? FECHA_REG { get; set; }

        public DateTime? FECHA_UPD { get; set; }

        public string EMAIL { get; set; }

        public string CELULAR { get; set; } = null!;

        public int? ID_PROFESION { get; set; } = null!;

         
    }
}
