using System.ComponentModel.DataAnnotations;

namespace CapacitaGRD_Admin.Entidades
{
    public class Configuracion
    {         
        public int ID_CONFIGURACION { get; set; }
        public string TEXTO_NOTIFICACION { get; set; } = null!;
        public string TEXTO_NOTIFICACION_ADJUNTO { get; set; } = null!;
        public string TEXTO_CERTIFICADO { get; set; } = null!;
        public string TEXTO_DOCUMENTO { get; set; } = null!;
        public string TEXTO_CONSTANCIA { get; set; } = null!;
    }
}
