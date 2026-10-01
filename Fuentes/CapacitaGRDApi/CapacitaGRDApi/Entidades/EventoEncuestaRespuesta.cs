using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Entidades
{
    // Respuesta real de UN participante a UNA pregunta de la encuesta de satisfacción de un
    // evento. No confundir con "EncuestaRespuesta" (esa es la lista maestra de alternativas
    // posibles, p.ej. "Muy satisfecho" / "Satisfecho"; esta tabla es el registro de qué
    // contestó cada persona).
    [PrimaryKey(nameof(ID_EVENTO_ENCUESTA_RESPUESTA))]
    public class EventoEncuestaRespuesta
    {
        public int ID_EVENTO_ENCUESTA_RESPUESTA { get; set; }
        public int ID_EVENTO { get; set; }
        public int ID_PERSONA { get; set; }
        public int ID_ENCUESTA { get; set; }
        public int ID_PREGUNTA { get; set; }
        public string RESPUESTA { get; set; } = null!;
        public DateTime FECHA_REG { get; set; }
    }
}
