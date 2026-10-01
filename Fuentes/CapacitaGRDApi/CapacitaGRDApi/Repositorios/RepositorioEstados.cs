using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEstados : IRepositorioEstados
    {
        private readonly DbContextClass DbContext;

        public RepositorioEstados(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Estado>> Listar()
        {
            return await DbContext.Estado.FromSqlRaw<Estado>("EXEC USP_MAE_ESTADO_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Estado>> Obtener(int id)        
        {             
            var param = new SqlParameter("@ID_ESTADO", id);
            var result = await Task.Run(() => DbContext.Estado.FromSqlRaw(@"exec USP_MAE_ESTADO_GET @ID_ESTADO", param).AsNoTracking().ToListAsync());            
            return  result;
        }
         

    }
}
