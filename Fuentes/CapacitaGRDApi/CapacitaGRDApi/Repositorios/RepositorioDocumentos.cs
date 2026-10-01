using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioDocumentos : IRepositorioDocumentos
    {
        private readonly DbContextClass DbContext;

        public RepositorioDocumentos(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Documento>> Listar()
        {
            return await DbContext.Documento.FromSqlRaw<Documento>("EXEC USP_MAE_TIPO_DOCUMENTO_REQUERIDO_SEL_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<List<Documento>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };  

            return await DbContext.Documento.FromSqlRaw<Documento>("EXEC USP_MAE_TIPO_DOCUMENTO_REQUERIDO_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<Documento>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_TIPO_DOCUMENTO_REQUERIDO", id);
            var result = await Task.Run(() => DbContext.Documento.FromSqlRaw(@"exec USP_MAE_TIPO_DOCUMENTO_REQUERIDO_GET @ID_TIPO_DOCUMENTO_REQUERIDO", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(Documento documento)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_TIPO_DOCUMENTO_REQUERIDO";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@NOMBRE", documento.NOMBRE));
            parameter.Add(LastID);

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_TIPO_DOCUMENTO_REQUERIDO_INS @NOMBRE, @ID_TIPO_DOCUMENTO_REQUERIDO OUTPUT", parameter.ToArray()));
            return LastID.Value as int? ?? default(int);

        }

        public async Task<int> Actualizar(Documento documento)
        {
            var parameter = new List<SqlParameter>();

            parameter.Add(new SqlParameter("@NOMBRE", documento.NOMBRE));
            parameter.Add(new SqlParameter("@ID_TIPO_DOCUMENTO_REQUERIDO", documento.ID_TIPO_DOCUMENTO_REQUERIDO));
            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_TIPO_DOCUMENTO_REQUERIDO_UPD @NOMBRE, @ID_TIPO_DOCUMENTO_REQUERIDO", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_MAE_TIPO_DOCUMENTO_REQUERIDO_DEL {id}"));
            return result;
        }

    }
}
