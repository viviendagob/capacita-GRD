using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioProfesiones : IRepositorioProfesiones
    {
        private readonly DbContextClass DbContext;

        public RepositorioProfesiones(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Profesion>> Listar()
        {
            return await DbContext.Profesion.FromSqlRaw<Profesion>("EXEC USP_MAE_PROFESION_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<List<Profesion>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Profesion.FromSqlRaw<Profesion>("EXEC USP_MAE_PROFESION_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Profesion>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_PROFESION", id);
            var result = await Task.Run(() => DbContext.Profesion.FromSqlRaw(@"exec USP_MAE_PROFESION_GET @ID_PROFESION", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(Profesion profesion)
        {
            var LastID = new SqlParameter
            {
                ParameterName = "@ID_PROFESION",
                Direction = ParameterDirection.Output,
                DbType = DbType.Int32
            };

            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@NOMBRE", profesion.NOMBRE),
                LastID
            };

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_PROFESION_INS @NOMBRE, @ID_PROFESION OUTPUT", parameter.ToArray()));
            return LastID.Value as int? ?? default(int);

        }

        public async Task<int> Actualizar(Profesion profesion)
        {
            var parameter = new List<SqlParameter>();

            parameter.Add(new SqlParameter("@NOMBRE", profesion.NOMBRE));
            parameter.Add(new SqlParameter("@ID_PROFESION", profesion.ID_PROFESION));
            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_PROFESION_UPD @NOMBRE, @ID_PROFESION", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_MAE_PROFESION_DEL {id}"));
            return result;

        }

    }
}
