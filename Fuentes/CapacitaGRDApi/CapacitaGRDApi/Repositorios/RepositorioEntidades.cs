using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Formats.Asn1;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEntidades : IRepositorioEntidades
    {
        private readonly DbContextClass DbContext;

        public RepositorioEntidades(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Entidad>> Listar()
        {
            return await DbContext.Entidad.FromSqlRaw<Entidad>("EXEC USP_MAE_ENTIDAD_SEL_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Entidad>> Obtener(int id)        
        {             
            var param = new SqlParameter("@ID_ENTIDAD", id);
            var result = await Task.Run(() => DbContext.Entidad.FromSqlRaw(@"exec USP_MAE_ENTIDAD_GET @ID_ENTIDAD", param).AsNoTracking().ToListAsync());            
            return  result;
        }
         

    }
}
