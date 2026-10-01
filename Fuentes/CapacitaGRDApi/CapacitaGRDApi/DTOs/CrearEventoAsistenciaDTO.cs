using CapacitaGRDApi.Entidades;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.DTOs
{
    public class CrearEventoAsistenciaDTO
    {
        public int ID_EVENTO { get; set; }
        public int ID_PERSONA { get; set; }
 
        public int ID_MODALIDAD { get; set; }

    }
}
