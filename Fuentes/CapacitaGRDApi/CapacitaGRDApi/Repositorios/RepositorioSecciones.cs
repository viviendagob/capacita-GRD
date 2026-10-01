using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioSecciones : IRepositorioSecciones
    {
        private readonly DbContextClass DbContext;

        public RepositorioSecciones(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Seccion>> Listar()
        {
            return await DbContext.Seccion.FromSqlRaw<Seccion>("EXEC USP_MAE_SECCION_SEL_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Seccion>> Obtener(int id)        
        {             
            var param = new SqlParameter("@ID_SECCION", id);
            var result = await Task.Run(() => DbContext.Seccion.FromSqlRaw(@"exec USP_MAE_SECCION_GET @ID_SECCION", param).AsNoTracking().ToListAsync());            
            return  result;
        }

        public async Task<int> Agregar(Seccion seccion)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_SECCION";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@NOMBRE", seccion.NOMBRE),
                LastID
            };

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_SECCION_INS @NOMBRE, @ID_SECCION OUTPUT", [..parameter]));
            return LastID.Value as int? ?? 0;

        }

        public async Task<int> Actualizar(Seccion seccion)
        {
            var parameter = new List<SqlParameter>
            {
                new("@NOMBRE", seccion.NOMBRE),
                new("@ID_SECCION", seccion.ID_SECCION)
            };
            return  await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_SECCION_UPD @NOMBRE, @ID_SECCION", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_MAE_SECCION_DEL {id}"));
            return result;

        }

    }
}
