using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Formats.Asn1;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioCargos : IRepositorioCargos
    {
        private readonly DbContextClass DbContext;

        public RepositorioCargos(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Cargo>> Listar()
        {
            return await DbContext.Cargo.FromSqlRaw<Cargo>("EXEC USP_MAE_CARGO_SEL_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<List<Cargo>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Cargo.FromSqlRaw<Cargo>("EXEC USP_MAE_CARGO_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<Cargo>> Obtener(int id)        
        {             
            var param = new SqlParameter("@ID_CARGO", id);
            var result = await Task.Run(() => DbContext.Cargo.FromSqlRaw(@"exec USP_MAE_CARGO_GET @ID_CARGO", param).AsNoTracking().ToListAsync());            
            return  result;
        }

        public async Task<int> Agregar(Cargo cargo)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_CARGO";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>();
            parameter.Add(new SqlParameter("@NOMBRE", cargo.NOMBRE));
            parameter.Add(LastID);

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_CARGO_INS @NOMBRE, @ID_CARGO OUTPUT", parameter.ToArray()));
            return LastID.Value as int? ?? default(int);

        }

        public async Task<int> Actualizar(Cargo cargo)
        {
            var parameter = new List<SqlParameter>();
            
            parameter.Add(new SqlParameter("@NOMBRE", cargo.NOMBRE));
            parameter.Add(new SqlParameter("@ID_CARGO", cargo.ID_CARGO));
            return  await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_MAE_CARGO_UPD @NOMBRE, @ID_CARGO", parameter.ToArray()));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_MAE_CARGO_DEL {id}"));
            return result;

        }

    }
}
