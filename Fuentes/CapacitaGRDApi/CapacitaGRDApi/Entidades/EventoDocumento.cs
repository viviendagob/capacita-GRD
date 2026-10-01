using CapacitaGRDApi.Util;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Entidades
{
    [PrimaryKey(nameof(ID_EVENTO), nameof(ID_TIPO_DOCUMENTO_REQUERIDO))]


    public class EventoDocumento : Registros
    { 

        public int ID_EVENTO { get; set; }
        public int ID_TIPO_DOCUMENTO_REQUERIDO { get; set; }

    }
}
