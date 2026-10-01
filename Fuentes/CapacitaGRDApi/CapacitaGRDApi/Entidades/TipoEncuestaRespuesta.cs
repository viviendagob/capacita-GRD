using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.Entidades
{
    public class TipoEncuestaRespuesta : Registros
    {
        [Key]
        public int ID_TIPO_ENCUESTA_PREGUNTA { get; set; }

        [StringLength(250)]
        public String NOMBRE { get; set; } = null!;

    }
}
