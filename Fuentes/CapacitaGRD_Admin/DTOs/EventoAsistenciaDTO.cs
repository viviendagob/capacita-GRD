 
namespace CapacitaGRD_Admin.DTOs
{
    public class EventoAsistenciaDTO
    {
        public int ID_EVENTO { get; set; }
        public int ID_PERSONA { get; set; }
        public DateTime FECHA { get; set; }
        public DateTime FECHA_REG { get; set; }
        public int ID_MODALIDAD { get; set; }

    }
}
