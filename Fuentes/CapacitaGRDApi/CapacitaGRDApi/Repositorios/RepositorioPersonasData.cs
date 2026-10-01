using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
 

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioPersonasData : IRepositorioPersonasData
    {
        private readonly DbContextClass DbContext;

        public RepositorioPersonasData(DbContextClass DbContext) {
            this.DbContext = DbContext;
        } 

        public async Task<IEnumerable<PersonaData>> Obtener(int id)
        {
            var param = new SqlParameter("@ID_PERSONA_DATA", id);
            var result = await Task.Run(() => DbContext.PersonaData.FromSqlRaw(@"exec USP_PERSONA_DATA_SEL @ID_PERSONA_DATA", param).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<IEnumerable<PersonaData>> ObtenerPorPersona(int idPersona)
        {
            var param = new SqlParameter("@ID_PERSONA", idPersona);
            return await Task.Run(() => DbContext.PersonaData
                .FromSqlRaw("SELECT * FROM PERSONA_DATA WHERE ID_PERSONA = @ID_PERSONA", param)
                .AsNoTracking()
                .ToListAsync());
        }

        public async Task<int> Agregar(PersonaData personaData)
        {
            var LastID = new SqlParameter();
            LastID.ParameterName = "@ID_PERSONA_DATA";
            LastID.Direction = ParameterDirection.Output;
            LastID.DbType = DbType.Int32;

            var parameter = new List<SqlParameter>
            {
                new("@ID_PERSONA", personaData.ID_PERSONA),
                new("@ID_PAIS_LABORA", personaData.ID_PAIS_LABORA),
                new ("@ID_ENTIDAD", personaData.ID_ENTIDAD),
                new ("@NOMBRE_ENTIDAD_OTRA", personaData.NOMBRE_ENTIDAD_OTRA),
                new ("@AREA_LABORA", personaData.AREA_LABORA),
                new ("@ID_CARGO", personaData.ID_CARGO),
                new ("@NOMBRE_CARGO_OTRA", personaData.NOMBRE_CARGO_OTRA),
                new ("@ID_DISTRITO", personaData.ID_DISTRITO),                
                LastID
            };

            var result = await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_PERSONA_DATA_INS @ID_PERSONA, @ID_PAIS_LABORA, @ID_ENTIDAD, @NOMBRE_ENTIDAD_OTRA, @AREA_LABORA, @ID_CARGO, @NOMBRE_CARGO_OTRA, @ID_DISTRITO, @ID_PERSONA_DATA OUTPUT", parameter));
            return LastID.Value as int? ?? 0;
        } 

        public async Task<int> Eliminar(int id)
        {
            var result = await Task.Run(() => DbContext.Database.ExecuteSqlInterpolatedAsync($"USP_PERSONA_DATA_DEL {id}"));
            return result;

        }

        // AREA_LABORA es texto libre (no tiene tabla maestra); esto da sugerencias ordenadas
        // alfabéticamente a partir de lo que otros participantes ya escribieron, para el
        // combo con opción de "añadir" del formulario de registro.
        public async Task<List<string>> ListarAreasLaborales()
        {
            return await DbContext.Database.SqlQuery<string>($@"
                SELECT DISTINCT AREA_LABORA AS Value
                FROM dbo.PERSONA_DATA
                WHERE AREA_LABORA IS NOT NULL AND LTRIM(RTRIM(AREA_LABORA)) <> ''
                ORDER BY AREA_LABORA ASC").ToListAsync();
        }

    }
}
