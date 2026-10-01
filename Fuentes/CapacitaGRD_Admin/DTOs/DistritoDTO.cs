using CapacitaGRD_Admin.Entidades;
 
namespace CapacitaGRD_Admin.DTOs
{
    public class DistritoDTO
    {
        public int ID_DISTRITO { get; set; }

        public string CODIGO_DEPARTAMENTO { get; set; } = null!;

        public string DEPARTAMENTO { get; set; } = null!;

        public string CODIGO_PROVINCIA { get; set; } = null!;

        public string PROVINCIA { get; set; } = null!;

        public string CODIGO_DISTRITO { get; set; } = null!;

        public string DISTRITO { get; set; } = null!;

        public string CAPITAL { get; set; } = null!;

        public string CATEGORIA { get; set; } = null!;

        public int ID_PAIS { get; set; }

        public IEnumerable<Pais> PAIS { get; set; } = null!;
    }
}
