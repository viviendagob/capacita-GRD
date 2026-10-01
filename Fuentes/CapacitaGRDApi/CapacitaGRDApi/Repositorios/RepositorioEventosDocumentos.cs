using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEventosDocumentos : IRepositorioEventosDocumentos
    {
        private readonly DbContextClass DbContext;
        public RepositorioEventosDocumentos(DbContextClass DbContext)
        {
            this.DbContext = DbContext;
        }

        public async Task<List<EventoDocumento>> Listar(int id)
        {
            var param = new SqlParameter("@ID_EVENTO", id);
            return await DbContext.EventoDocumento.FromSqlRaw<EventoDocumento>("EXEC USP_EVENTO_DOCUMENTO_SEL @ID_EVENTO", param).AsNoTracking().ToListAsync();
        }
         

        public async Task<int> Agregar(EventoDocumento documento)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", documento.ID_EVENTO),
                new("@ID_TIPO_DOCUMENTO_REQUERIDO", documento.ID_TIPO_DOCUMENTO_REQUERIDO),                
            };

            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"EXEC USP_EVENTO_DOCUMENTO_INS @ID_EVENTO , @ID_TIPO_DOCUMENTO_REQUERIDO ", parameter));
        }


        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_EVENTO_DOCUMENTO_DEL {id}"));
            return result;
        }

    }
}
