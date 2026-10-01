using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEncuestas : IRepositorioEncuestas
    {
        private readonly DbContextClass DbContext;

        public RepositorioEncuestas(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Encuesta>> Listar()
        {
            return await DbContext.Encuesta.FromSqlRaw<Encuesta>("EXEC USP_ENCUESTA_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<List<Encuesta>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Encuesta.FromSqlRaw<Encuesta>("EXEC USP_MAE_ENCUESTA_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Encuesta>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_ENCUESTA", id);
            var result = await Task.Run(() => DbContext.Encuesta.FromSqlRaw(@"exec USP_ENCUESTA_GET @ID_ENCUESTA", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(Encuesta encuesta)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_ENCUESTA";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@NOMBRE", encuesta.NOMBRE));
            parameter.Add(new SqlParameter("@USER_REG", encuesta.USER_REG));
      
            parameter.Add(LastID);

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_ENCUESTA_INS @NOMBRE, @USER_REG, @ID_ENCUESTA OUTPUT", parameter.ToArray()));
            return LastID.Value as int? ?? default(int);
             
        }

        public async Task<int> Actualizar(Encuesta encuesta)
        {

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@NOMBRE", encuesta.NOMBRE));
             parameter.Add(new SqlParameter("@USER_UPD", encuesta.USER_UPD));
            parameter.Add(new SqlParameter("@ID_ENCUESTA", encuesta.ID_ENCUESTA));

            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_ENCUESTA_UPD @NOMBRE, @USER_UPD, @ID_ENCUESTA", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_ENCUESTA_DEL {id}"));
            return result;

        }

        public async Task<int> EliminarRespuestas(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_ENCUESTA_RESPUESTA_DEL_ALL {id}"));
            return result;

        }

    }
}
