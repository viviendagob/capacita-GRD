using CapacitaGRDApi.Entidades;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.DTOs
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
