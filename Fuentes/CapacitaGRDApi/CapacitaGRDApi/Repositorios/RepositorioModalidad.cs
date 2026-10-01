using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioModalidad : IRepositorioModalidad
    {
        private readonly DbContextClass DbContext;

        public RepositorioModalidad(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Modalidad>> Listar()
        {
            return await DbContext.Modalidad.FromSqlRaw<Modalidad>("EXEC USP_MAE_MODALIDAD_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Modalidad>> Obtener(int id)        
        {             
            var param = new SqlParameter("@ID_MODALIDAD", id);
            var result = await Task.Run(() => DbContext.Modalidad.FromSqlRaw(@"exec USP_MAE_MODALIDAD_GET @ID_MODALIDAD", param).AsNoTracking().ToListAsync());            
            return  result;
        }
         

    }
}
