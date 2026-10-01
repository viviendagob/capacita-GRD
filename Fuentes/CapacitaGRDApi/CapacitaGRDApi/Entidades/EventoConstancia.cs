using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Entidades
{
    [PrimaryKey(nameof(ID_CONSTANCIA))]
    public class EventoConstancia
    {
        public int ID_CONSTANCIA { get; set; }
        public int ID_EVENTO { get; set; }
        public int ID_PERSONA { get; set; }
        public string CODIGO { get; set; } = null!;
        public string ESTADO { get; set; } = null!; // GENERADA | ANULADA
        public string? URL_PDF { get; set; }
        public DateTime FECHA_GENERACION { get; set; }
        public string? USER_GENERACION { get; set; }
        public DateTime? FECHA_ULTIMA_DESCARGA { get; set; }
        public int NUM_DESCARGAS { get; set; }
        public string? USER_ANULACION { get; set; }
        public DateTime? FECHA_ANULACION { get; set; }
        public string? MOTIVO_ANULACION { get; set; }
    }
}
