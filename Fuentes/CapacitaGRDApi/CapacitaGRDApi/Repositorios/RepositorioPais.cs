using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioPais : IRepositorioPais
    {
        private readonly DbContextClass DbContext;

        public RepositorioPais(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }
        public async Task<List<Pais>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Pais.FromSqlRaw<Pais>("EXEC USP_MAE_PAIS_PAGINADOR @ORDER, @WHERE, @LIMIT", [..parameter]).AsNoTracking().ToListAsync();
        }
         
        public async Task<List<Pais>> Listar()
        {
            return await DbContext.Pais.FromSqlRaw<Pais>("EXEC USP_MAE_PAIS_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Pais>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_PAIS", id);
            var result = await Task.Run(() => DbContext.Pais.FromSqlRaw(@"exec USP_MAE_PAIS_GET @ID_PAIS", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(Pais pais)
        {
            var LastID = new SqlParameter
            {
                ParameterName = "@ID_PAIS",
                Direction = ParameterDirection.Output,
                DbType = DbType.Int32
            };

            var parameter = new List<SqlParameter>
            {
                new("@NOMBRE", pais.NOMBRE),
                new("@ESLOCAL", pais.ESLOCAL),
                new("@COD_PAIS", pais.COD_PAIS),
                LastID
            };

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_PAIS_INS @NOMBRE, @ESLOCAL, @COD_PAIS, @ID_PAIS OUTPUT", parameter.ToArray()));
            return LastID.Value as int? ?? default(int);

        }

        public async Task<int> Actualizar(Pais pais)
        {
            var parameter = new List<SqlParameter>();

            parameter.Add(new SqlParameter("@NOMBRE", pais.NOMBRE));
            parameter.Add(new SqlParameter("@ESLOCAL", pais.ESLOCAL));
            parameter.Add(new SqlParameter("@COD_PAIS", pais.COD_PAIS));
            parameter.Add(new SqlParameter("@ID_PAIS", pais.ID_PAIS));
            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_PAIS_UPD @NOMBRE, @ESLOCAL, @COD_PAIS, @ID_PAIS", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_MAE_PAIS_DEL {id}"));
            return result;

        }

    }
}
