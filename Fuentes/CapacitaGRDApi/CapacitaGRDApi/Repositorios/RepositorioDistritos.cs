using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioDistritos : IRepositorioDistritos
    {
        private readonly DbContextClass DbContext;

        public RepositorioDistritos(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Distrito>> Listar()
        {
            return await DbContext.Distrito.FromSqlRaw<Distrito>("EXEC USP_MAE_DISTRITO_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<List<Distrito>> Paginar(Filter filter)
        {
            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Distrito.FromSqlRaw<Distrito>("EXEC USP_MAE_DISTRITO_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Distrito>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_DISTRITO", id);
            var result = await Task.Run(() => DbContext.Distrito.FromSqlRaw(@"exec USP_MAE_DISTRITO_GET @ID_DISTRITO", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(Distrito distrito)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_DISTRITO";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@CODIGO_DEPARTAMENTO", distrito.CODIGO_DEPARTAMENTO));
            parameter.Add(new SqlParameter("@DEPARTAMENTO", distrito.DEPARTAMENTO));
            parameter.Add(new SqlParameter("@CODIGO_PROVINCIA", distrito.CODIGO_PROVINCIA));
            parameter.Add(new SqlParameter("@PROVINCIA", distrito.PROVINCIA));
            parameter.Add(new SqlParameter("@CODIGO_DISTRITO", distrito.CODIGO_DISTRITO));
            parameter.Add(new SqlParameter("@DISTRITO", distrito.DISTRITO));
            parameter.Add(new SqlParameter("@CAPITAL", distrito.CAPITAL));
            parameter.Add(new SqlParameter("@CATEGORIA", distrito.CATEGORIA));
            parameter.Add(new SqlParameter("@CAPITAL", distrito.CAPITAL));
            parameter.Add(new SqlParameter("@ID_PAIS", distrito.ID_PAIS));

            parameter.Add(LastID);

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_DISTRITO_INS @CODIGO_DEPARTAMENTO, @DEPARTAMENTO, @CODIGO_PROVINCIA, @PROVINCIA, @CODIGO_DISTRITO, @DISTRITO, @CAPITAL, @CATEGORIA, @ID_PAIS, @ID_DISTRITO OUTPUT", parameter.ToArray()));
            return LastID.Value as int? ?? default(int);

        }

        public async Task<int> Actualizar(Distrito distrito)
        {
            var parameter = new List<SqlParameter>();

            parameter.Add(new SqlParameter("@CODIGO_DEPARTAMENTO", distrito.CODIGO_DEPARTAMENTO));
            parameter.Add(new SqlParameter("@DEPARTAMENTO", distrito.DEPARTAMENTO));
            parameter.Add(new SqlParameter("@CODIGO_PROVINCIA", distrito.CODIGO_PROVINCIA));
            parameter.Add(new SqlParameter("@PROVINCIA", distrito.PROVINCIA));
            parameter.Add(new SqlParameter("@CODIGO_DISTRITO", distrito.CODIGO_DISTRITO));
            parameter.Add(new SqlParameter("@DISTRITO", distrito.DISTRITO));
            parameter.Add(new SqlParameter("@CAPITAL", distrito.CAPITAL));
            parameter.Add(new SqlParameter("@CATEGORIA", distrito.CATEGORIA));
            parameter.Add(new SqlParameter("@CAPITAL", distrito.CAPITAL));
            parameter.Add(new SqlParameter("@ID_PAIS", distrito.ID_PAIS));

            parameter.Add(new SqlParameter("@ID_DISTRITO", distrito.ID_DISTRITO));

            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_DISTRITO_UPD @CODIGO_DEPARTAMENTO, @DEPARTAMENTO, @CODIGO_PROVINCIA, @PROVINCIA, @CODIGO_DISTRITO, @DISTRITO, @CAPITAL, @CATEGORIA, @ID_PAIS, @ID_DISTRITO", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_MAE_DISTRITO_DEL {id}"));
            return result;

        }

    }
}
