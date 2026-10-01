using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Formats.Asn1;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioTipoDocumentos : IRepositorioTipoDocumentos
    {
        private readonly DbContextClass DbContext;

        public RepositorioTipoDocumentos(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<TipoDocumento>> Listar()
        {
            return await DbContext.TipoDocumento.FromSqlRaw<TipoDocumento>("EXEC USP_MAE_TIPO_DOCUMENTO_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TipoDocumento>> Obtener(int id)        
        {             
            var param = new SqlParameter("@ID_TIPO_DOCUMENTO", id);
            var result = await Task.Run(() => DbContext.TipoDocumento.FromSqlRaw(@"exec USP_MAE_TIPO_DOCUMENTO_GET @ID_TIPO_DOCUMENTO", param).AsNoTracking().ToListAsync());            
            return  result;
        }
         

    }
}
