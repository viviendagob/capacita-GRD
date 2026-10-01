using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Formats.Asn1;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly DbContextClass DbContext;

        public RepositorioUsuarios(DbContextClass DbContext)
        {
            this.DbContext = DbContext;
        }
        public async Task<List<Usuarios>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Usuarios.FromSqlRaw<Usuarios>("EXEC USP_MAE_USUARIO_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }

        public async Task<List<Usuarios>> Listar()
        {
            return await DbContext.Usuarios.FromSqlRaw<Usuarios>("EXEC USP_MAE_USUARIO_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Usuarios>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_USUARIO", id);
            var result = await Task.Run(() => DbContext.Usuarios.FromSqlRaw(@"exec USP_MAE_USUARIO_GET @ID_USUARIO", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(Usuarios usuario)
        {
            var LastID = new SqlParameter
            {
                ParameterName = "@ID_USUARIO",
                Direction = ParameterDirection.Output,
                DbType = DbType.Int32
            };

            var parameter = new List<SqlParameter>
            {
                new("@USUARIO", usuario.USUARIO),
                new("@CLAVE", usuario.CLAVE),
                new("@ESTADO", usuario.ESTADO),
                LastID
            };

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_USUARIO_INS @USUARIO, @CLAVE, @ESTADO, @ID_USUARIO OUTPUT", parameter.ToArray()));
            return LastID.Value as int? ?? default(int);

        }

        public async Task<int> Actualizar(Usuarios usuario)
        {
            var parameter = new List<SqlParameter>();

            parameter.Add(new SqlParameter("@USUARIO", usuario.USUARIO));
            parameter.Add(new SqlParameter("@CLAVE", usuario.CLAVE));
            parameter.Add(new SqlParameter("@ESTADO", usuario.ESTADO));
            parameter.Add(new SqlParameter("@ID_USUARIO", usuario.ID_USUARIO));
            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_USUARIO_UPD @USUARIO, @CLAVE, @ESTADO, @ID_USUARIO", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_MAE_USUARIO_DEL {id}"));
            return result;

        }

        public async Task<IEnumerable<Usuario>> Busqueda(string usuario, string clave)
        {

            var parameter = new List<SqlParameter>
            {
                new("@USUARIO", usuario),
                new("@CLAVE", clave)
            };

            var result = await Task.Run(() => DbContext.Usuario.FromSqlRaw(@"EXEC USP_MAE_USUARIO_BUSQUEDA @USUARIO,@CLAVE", [.. parameter]).AsNoTracking().ToListAsync());
            return result;
        }


    }
}
