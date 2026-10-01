using CapacitaGRDApi.Util;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Entidades
{
    [PrimaryKey(nameof(ID_EVENTO), nameof(ID_PERSONA), nameof(FECHA))]


    public class EventoAsistencia : Registros
    { 

        public int ID_EVENTO { get; set; }
        public int ID_PERSONA { get; set; }
        public DateTime FECHA { get; set; }
        public DateTime FECHA_REG { get; set; }
        public int ID_MODALIDAD { get; set; }
         
    }
}
