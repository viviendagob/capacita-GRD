using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioCuestionarioPreguntas : IRepositorioCuestionarioPreguntas
    {
        private readonly DbContextClass DbContext;

        public RepositorioCuestionarioPreguntas(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<CuestionarioPregunta>> Listar(int idCuestionario)
        {
            var param = new SqlParameter("@ID_CUESTIONARIO", idCuestionario);
            return await DbContext.CuestionarioPregunta.FromSqlRaw<CuestionarioPregunta>(@"EXEC USP_CUESTIONARIO_PREGUNTA_ALL @ID_CUESTIONARIO", param).AsNoTracking().ToListAsync();
        }

        public async Task<int> Agregar(CuestionarioPregunta pregunta)
        {
            var LastID = new SqlParameter
            {
                ParameterName = "@ID_CUESTIONARIO_PREGUNTA",
                Direction = ParameterDirection.Output,
                DbType = DbType.Int32
            };

            var parameter = new List<SqlParameter>
            {
                new("@NOMBRE", pregunta.NOMBRE),
                new("@PESO", pregunta.PESO),
                new("@ID_CUESTIONARIO", pregunta.ID_CUESTIONARIO),
                LastID
            };

            await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_CUESTIONARIO_PREGUNTA_INS @NOMBRE, @PESO, @ID_CUESTIONARIO, @ID_CUESTIONARIO_PREGUNTA OUTPUT", parameter));
            return LastID.Value as int? ?? 0;
        }

        public async Task<int> EliminarPorCuestionario(int idCuestionario)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_CUESTIONARIO_PREGUNTA_DEL_ALL {idCuestionario}"));
            return result;
        }

    }
}
