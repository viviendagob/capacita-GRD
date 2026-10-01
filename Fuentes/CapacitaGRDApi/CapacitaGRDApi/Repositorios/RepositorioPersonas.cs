using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static CapacitaGRDApi.Util.Paginado;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioPersonas : IRepositorioPersonas
    {
        private readonly DbContextClass DbContext;

        public RepositorioPersonas(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }

        public async Task<List<Persona>> Listar()
        {
            return await DbContext.Persona.FromSqlRaw<Persona>("EXEC USP_PERSONA_ALL").AsNoTracking().ToListAsync();
        }

        public async Task<List<Persona>> Paginar(Filter filter)
        {

            var parameter = new List<SqlParameter>
            {
                new("@WHERE", filter.Where),
                new("@ORDER", filter.Order),
                new("@LIMIT", filter.Limit)
            };

            return await DbContext.Persona.FromSqlRaw<Persona>("EXEC USP_MAE_PERSONA_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }

        public async Task<List<Persona>> ListarFiltrado(string where, string order)
        {
            var parameter = new List<SqlParameter>
            {
                new("@WHERE", where),
                new("@ORDER", order),
                new("@LIMIT", " FILA BETWEEN 1 AND 999999")
            };

            return await DbContext.Persona.FromSqlRaw<Persona>("EXEC USP_MAE_PERSONA_PAGINADOR @ORDER, @WHERE, @LIMIT", [.. parameter]).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Persona>> Existe(int tipo, string documento)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_TIPO_DOCUMENTO", tipo),
                new("@NUM_DOCUMENTO", documento)             
            };

            var result = await Task.Run(() => DbContext.Persona.FromSqlRaw(@"exec USP_PERSONA_EXISTE @ID_TIPO_DOCUMENTO, @NUM_DOCUMENTO", parameter.ToArray()).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<IEnumerable<Persona>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_PERSONA", id);
            var result = await Task.Run(() => DbContext.Persona.FromSqlRaw(@"exec USP_PERSONA_GET @ID_PERSONA", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(Persona persona)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_PERSONA";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>
            {
                new("@ID_TIPO_DOCUMENTO", persona.ID_TIPO_DOCUMENTO),
                new("@NUM_DOCUMENTO", persona.NUM_DOCUMENTO),
                new ("@NOMBRES", persona.NOMBRES),
                new ("@APELLIDO_PATERNO", persona.APELLIDO_PATERNO),
                new ("@APELLIDO_MATERNO", persona.APELLIDO_MATERNO),
                new ("@SEXO", persona.SEXO),
                new ("@ID_PAIS_NACIMIENTO", persona.ID_PAIS_NACIMIENTO),
                new ("@FECHA_NACIMIENTO", (object?)persona.FECHA_NACIMIENTO ?? DBNull.Value),
                new ("@EMAIL", persona.EMAIL),
                new ("@CELULAR", persona.CELULAR),
                new ("@ID_PROFESION", (object?)persona.ID_PROFESION ?? DBNull.Value),
                new ("@VALIDADO_PIDE", persona.VALIDADO_PIDE),
                LastID
            };

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_PERSONA_INS @ID_TIPO_DOCUMENTO, @NUM_DOCUMENTO, @NOMBRES, @APELLIDO_PATERNO, @APELLIDO_MATERNO, @SEXO, @ID_PAIS_NACIMIENTO, @FECHA_NACIMIENTO, @EMAIL, @CELULAR, @ID_PROFESION, @VALIDADO_PIDE, @ID_PERSONA OUTPUT", parameter));
            return LastID.Value as int? ?? 0;

        }

        public async Task<int> Actualizar(Persona persona)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_TIPO_DOCUMENTO", persona.ID_TIPO_DOCUMENTO),
                new("@NUM_DOCUMENTO", persona.NUM_DOCUMENTO),
                new ("@NOMBRES", persona.NOMBRES),
                new ("@APELLIDO_PATERNO", persona.APELLIDO_PATERNO),
                new ("@APELLIDO_MATERNO", persona.APELLIDO_MATERNO),
                new ("@SEXO", persona.SEXO),
                new ("@ID_PAIS_NACIMIENTO", persona.ID_PAIS_NACIMIENTO),
                new ("@FECHA_NACIMIENTO", (object?)persona.FECHA_NACIMIENTO ?? DBNull.Value),
                new ("@EMAIL", persona.EMAIL),
                new ("@CELULAR", persona.CELULAR),
                new ("@ID_PROFESION", (object?)persona.ID_PROFESION ?? DBNull.Value),
                new ("@VALIDADO_PIDE", persona.VALIDADO_PIDE),
                new ("@ID_PERSONA", persona.ID_PERSONA),
            };
            
            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_PERSONA_UPD @ID_TIPO_DOCUMENTO, @NUM_DOCUMENTO, @NOMBRES, @APELLIDO_PATERNO, @APELLIDO_MATERNO, @SEXO, @ID_PAIS_NACIMIENTO, @FECHA_NACIMIENTO, @EMAIL, @CELULAR, @ID_PROFESION, @VALIDADO_PIDE, @ID_PERSONA ", parameter));
        }

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_PERSONA_DEL {id}"));
            return result;

        }

        public async Task<int> Reiniciar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE PERSONA SET FECHA_NACIMIENTO = NULL, VALIDADO_PIDE = 'N' WHERE ID_PERSONA = {id}"));
            return result;
        }

    }
}
