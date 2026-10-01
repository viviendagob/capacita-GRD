using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class CuestionarioPreguntaRespuesta : Registros
    {

        [Key]
        public int ID_PREGUNTA_RESPUESTA { get; set; }

        [StringLength(250)]
        public string NOMBRE { get; set; } = null!;

        public int ID_CUESTIONARIO_PREGUNTA { get; set; }

    }
}
