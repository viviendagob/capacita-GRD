using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioConfiguracion : IRepositorioConfiguracion
    {
        private readonly DbContextClass DbContext;

        public RepositorioConfiguracion(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }
    
        public async Task<IEnumerable<Configuracion>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_CONFIGURACION", id);
            var result = await Task.Run(() => DbContext.Configuracion.FromSqlRaw(@"exec USP_CONFIGURACION_SEL @ID_CONFIGURACION", param).AsNoTracking().ToListAsync());
            return result;
        } 

        public async Task<int> Actualizar(Configuracion configuracion)
        {
            var parameter = new List<SqlParameter>
            {
                new("@TEXTO_NOTIFICACION", configuracion.TEXTO_NOTIFICACION),
                new("@TEXTO_NOTIFICACION_ADJUNTO", configuracion.TEXTO_NOTIFICACION_ADJUNTO),
                new("@TEXTO_CERTIFICADO", configuracion.TEXTO_CERTIFICADO),
                new("@TEXTO_DOCUMENTO", configuracion.TEXTO_DOCUMENTO),
                new("@TEXTO_CONSTANCIA", configuracion.TEXTO_CONSTANCIA),
                new("@ID_CONFIGURACION", configuracion.ID_CONFIGURACION)
            };
            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_CONFIGURACION_UPD @TEXTO_NOTIFICACION, @TEXTO_NOTIFICACION_ADJUNTO, @TEXTO_CERTIFICADO, @TEXTO_DOCUMENTO, @TEXTO_CONSTANCIA,  @ID_CONFIGURACION", [..parameter]));
        }

       
    }
}
