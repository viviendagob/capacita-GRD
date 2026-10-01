using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioTipoEventos : IRepositorioTipoEventos
    {
        private readonly DbContextClass DbContext;

        public RepositorioTipoEventos(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<TipoEvento>> Listar()
        {
            return await DbContext.TipoEvento.FromSqlRaw<TipoEvento>("EXEC USP_MAE_TIPO_EVENTO_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TipoEvento>> Obtener(int id)        
        {             
            var param = new SqlParameter("@ID_TIPO_EVENTO", id);
            var result = await Task.Run(() => DbContext.TipoEvento.FromSqlRaw(@"exec USP_MAE_TIPO_EVENTO_GET @ID_TIPO_EVENTO", param).AsNoTracking().ToListAsync());            
            return  result;
        }
         

    }
}
