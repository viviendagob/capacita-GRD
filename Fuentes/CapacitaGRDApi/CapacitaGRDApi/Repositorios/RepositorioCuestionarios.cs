using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioCuestionarios : IRepositorioCuestionarios
    {
        private readonly DbContextClass DbContext;

        public RepositorioCuestionarios(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Cuestionario>> Listar()
        {
            return await DbContext.Cuestionario.FromSqlRaw<Cuestionario>("EXEC USP_CUESTIONARIO_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Cuestionario>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_CUESTIONARIO", id);
            var result = await Task.Run(() => DbContext.Cuestionario.FromSqlRaw(@"exec USP_CUESTIONARIO_GET @ID_CUESTIONARIO", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<List<Cuestionario>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Cuestionario.FromSqlRaw<Cuestionario>("EXEC USP_MAE_CUESTIONARIO_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }

        public async Task<int> Agregar(Cuestionario cuestionario)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_CUESTIONARIO";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@NOMBRE", cuestionario.NOMBRE));
            parameter.Add(new SqlParameter("@USER_REG", cuestionario.USER_REG));
      
            parameter.Add(LastID);

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_CUESTIONARIO_INS @NOMBRE, @USER_REG, @ID_CUESTIONARIO OUTPUT", parameter.ToArray()));
            return LastID.Value as int? ?? default(int);
             
        }

        public async Task<int> Actualizar(Cuestionario cuestionario)
        {

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@NOMBRE", cuestionario.NOMBRE));
             parameter.Add(new SqlParameter("@USER_UPD", cuestionario.USER_UPD));
            parameter.Add(new SqlParameter("@ID_CUESTIONARIO", cuestionario.ID_CUESTIONARIO));

            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_CUESTIONARIO_UPD @NOMBRE, @USER_UPD, @ID_CUESTIONARIO", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_CUESTIONARIO_DEL {id}"));
            return result;

        }

    }
}
