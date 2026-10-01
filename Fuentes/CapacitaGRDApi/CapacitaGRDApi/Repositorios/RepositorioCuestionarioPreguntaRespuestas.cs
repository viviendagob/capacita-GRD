using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioCuestionarioPreguntaRespuestas : IRepositorioCuestionarioPreguntaRespuestas
    {
        private readonly DbContextClass DbContext;

        public RepositorioCuestionarioPreguntaRespuestas(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<CuestionarioPreguntaRespuesta>> Listar(int idCuestionarioPregunta)
        {
            var param = new SqlParameter("@ID_CUESTIONARIO_PREGUNTA", idCuestionarioPregunta);
            return await DbContext.CuestionarioPreguntaRespuesta.FromSqlRaw<CuestionarioPreguntaRespuesta>(@"EXEC USP_CUESTIONARIO_PREGUNTA_RESPUESTA_ALL @ID_CUESTIONARIO_PREGUNTA", param).AsNoTracking().ToListAsync();
        }

        public async Task<int> Agregar(CuestionarioPreguntaRespuesta respuesta)
        {
            var LastID = new SqlParameter
            {
                ParameterName = "@ID_PREGUNTA_RESPUESTA",
                Direction = ParameterDirection.Output,
                DbType = DbType.Int32
            };

            var parameter = new List<SqlParameter>
            {
                new("@NOMBRE", respuesta.NOMBRE),
                new("@ID_CUESTIONARIO_PREGUNTA", respuesta.ID_CUESTIONARIO_PREGUNTA),
                LastID
            };

            await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_CUESTIONARIO_PREGUNTA_RESPUESTA_INS @NOMBRE, @ID_CUESTIONARIO_PREGUNTA, @ID_PREGUNTA_RESPUESTA OUTPUT", parameter));
            return LastID.Value as int? ?? 0;
        }

    }
}
