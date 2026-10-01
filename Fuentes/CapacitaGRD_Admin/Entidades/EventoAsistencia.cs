
namespace CapacitaGRD_Admin.Entidades
{
    
    public class EventoAsistencia  
    { 
        public int ID_EVENTO { get; set; }
        public int ID_PERSONA { get; set; }
        public DateTime FECHA { get; set; }
        public DateTime FECHA_REG { get; set; }
        public int ID_MODALIDAD { get; set; }
         
    }
}
