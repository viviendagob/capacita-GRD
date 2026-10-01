namespace CapacitaGRD_Admin.Entidades
{
    public class EventoFecha  
    {

         public int ID_EVENTO { get; set; }

         public DateTime FECHA { get; set; }

        public TimeOnly? HORA_INICIO { get; set; }

        public TimeOnly? HORA_FIN { get; set; }

        public string ES_ENCUESTA { get; set; } = null!;

        public int? ID_ENCUESTA { get; set; } = null!;

        public string ES_CUESTIONARIO { get; set; } = null!;

        public int? ID_CUESTIONARIO { get; set; } = null!;

    }
}
