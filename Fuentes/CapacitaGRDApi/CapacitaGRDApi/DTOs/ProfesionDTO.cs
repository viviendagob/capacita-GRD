using CapacitaGRDApi.Util;
using System.ComponentModel.DataAnnotations;

namespace CapacitaGRDApi.DTOs
{
    public class ProfesionDTO 
    {
        public int ID_PROFESION { get; set; }

        public String NOMBRE { get; set; } = null!;
    }
}
