using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class CuestionarioPregunta : Registros
    {

        [Key]
        public int ID_CUESTIONARIO_PREGUNTA { get; set; }

        [StringLength(250)]
        public string NOMBRE { get; set; } = null!;

        public int PESO { get; set; }

        public int ID_CUESTIONARIO { get; set; }

    }
}
