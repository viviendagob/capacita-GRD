using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEncuestaRespuestas : IRepositorioEncuestaRespuestas
    {
        private readonly DbContextClass DbContext;

        public RepositorioEncuestaRespuestas(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<EncuestaRespuesta>> Listar(int id)
        {
            var param = new SqlParameter("@ID_ENCUESTA", id);
            return await DbContext.EncuestaRespuesta.FromSqlRaw<EncuestaRespuesta>(@"EXEC USP_ENCUESTA_RESPUESTA_ALL @ID_ENCUESTA", param).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<EncuestaRespuesta>> Obtener(int idEncuesta, int id)        
        {             
            var param = new SqlParameter("@ID_RESPUESTA", id);
            var result = await Task.Run(() => DbContext.EncuestaRespuesta.FromSqlRaw(@"EXEC USP_ENCUESTA_RESPUESTA_GET @ID_RESPUESTA", param).AsNoTracking().ToListAsync());            
            return  result;
        }

        public async Task<int> Agregar(EncuestaRespuesta encuestaRespuesta)
        {
            var LastID = new SqlParameter
            {
                ParameterName = "@ID_RESPUESTA",
                Direction = ParameterDirection.Output,
                DbType = DbType.Int32
            };

            var parameter = new List<SqlParameter>
            {
                new("@NOMBRE", encuestaRespuesta.NOMBRE),
                new("@RESPUESTAS", encuestaRespuesta.RESPUESTAS),
                new("@ID_ENCUESTA", encuestaRespuesta.ID_ENCUESTA),
                new("@ID_TIPO_ENCUESTA_PREGUNTA", encuestaRespuesta.ID_TIPO_ENCUESTA_PREGUNTA),
                LastID
            };
             

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_ENCUESTA_RESPUESTA_INS @NOMBRE, @RESPUESTAS, @ID_ENCUESTA , @ID_TIPO_ENCUESTA_PREGUNTA, @ID_RESPUESTA OUTPUT", parameter));
            return LastID.Value as int? ?? 0;

        }

        public async Task<int> Actualizar(EncuestaRespuesta encuestaRespuesta)
        {
            var parameter = new List<SqlParameter>
            {
                new("@NOMBRE", encuestaRespuesta.NOMBRE),
                new("@RESPUESTAS", encuestaRespuesta.RESPUESTAS),
                new("@ID_ENCUESTA", encuestaRespuesta.ID_ENCUESTA),
                new("@ID_TIPO_ENCUESTA_PREGUNTA", encuestaRespuesta.ID_TIPO_ENCUESTA_PREGUNTA),
                new("@ID_RESPUESTA", encuestaRespuesta.ID_RESPUESTA),
            };

            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_ENCUESTA_RESPUESTA_UPD @NOMBRE, @RESPUESTAS, @ID_ENCUESTA , @ID_TIPO_ENCUESTA_PREGUNTA, @ID_RESPUESTA", parameter));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_ENCUESTA_RESPUESTA_DEL {id}"));
            return result;

        }

    }
}
