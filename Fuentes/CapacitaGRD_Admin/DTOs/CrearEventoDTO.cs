using System.ComponentModel.DataAnnotations;

namespace CapacitaGRD_Admin.DTOs
{
    public class CrearEventoDTO
    {

        public string NOMBRE { get; set; } = null!;

        public DateTime FECHA_INICIO { get; set; }

        public DateTime FECHA_FIN { get; set; }

        public string NOMBRE_LUGAR { get; set; } = null!;

        public int ID_ESTADO { get; set; }

        public int ID_UBIGEO { get; set; }

        public string BANNER { get; set; } = null!;

        public int NUM_PARTICIPANTES { get; set; }

        public string DESCRIPCION { get; set; } = null!;

        public string ENLACE_WHATSAPP { get; set; } = null!;

        public string? HORA_INICIO { get; set; }

        public string? HORA_FIN { get; set; }

        public string RED_SOCIAL { get; set; } = null!;

        public int ID_TIPO_EVENTO { get; set; }

        public int ID_MODALIDAD { get; set; }

        public List<CrearEventoFechaDTO>? FECHAS { get; set; } = null;

        public string GENERA_TICKET { get; set; } = null!;
        public string ENVIA_CORREO { get; set; } = null!;

        public int? ID_ENCUESTA { get; set; }
        public int? ID_CUESTIONARIO { get; set; }

        public string FORMATO { get; set; } = null!;

        public string REQUIERE_DOCUMENTO { get; set; } = null!;

        public List<int>? Documentos { get; set; } = null;

    }
}
