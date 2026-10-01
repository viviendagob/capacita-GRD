using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEventosParticipantes : IRepositorioEventosParticipantes
    {
        private readonly DbContextClass DbContext;
     
        public RepositorioEventosParticipantes(DbContextClass DbContext)
         {
            this.DbContext = DbContext;
        }
 

        public async Task<IEnumerable<EventoParticipante>> Existe(int idEvento, int idPersona)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", idEvento),
                new("@ID_PERSONA", idPersona)
            };

            var result = await Task.Run(() => DbContext.EventoParticipante.FromSqlRaw(@"exec USP_EVENTO_PARTICIPANTE_SEL  @ID_EVENTO, @ID_PERSONA", [.. parameter]).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<List<EventoParticipante>> Eventos(int idPersona)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_PERSONA", idPersona)
            };

            var result = await Task.Run(() => DbContext.EventoParticipante.FromSqlRaw<EventoParticipante>(@"exec USP_EVENTO_PARTICIPANTE_EVENTOS_SEL  @ID_PERSONA", [.. parameter]).AsNoTracking().ToListAsync());
            return result;
        }

        public async Task<int> Agregar(EventoParticipante eventoParticipante)
        {           
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", eventoParticipante.ID_EVENTO),
                new("@ID_PERSONA_DATA", eventoParticipante.ID_PERSONA_DATA),
                new("@ID_MODALIDAD", eventoParticipante.ID_MODALIDAD),
                new("@LATITUD", eventoParticipante.LATITUD as object ?? DBNull.Value),
                new("@LONGITUD", eventoParticipante.LONGITUD as object ?? DBNull.Value),
                new("@ID_PERSONA", eventoParticipante.ID_PERSONA),               
            };
            
            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_EVENTO_PARTICIPANTE_INS @ID_EVENTO, @ID_PERSONA_DATA, @ID_MODALIDAD, @LATITUD, @LONGITUD, @ID_PERSONA", [.. parameter]));
        }

        public async Task<List<ReporteInscritoDTO>> ReporteInscritos(int idEvento)
        {
            return await DbContext.Database.SqlQuery<ReporteInscritoDTO>($@"
                SELECT
                    ISNULL(TD.NOMBRE, '') AS TIPO_DOCUMENTO,
                    P.NUM_DOCUMENTO AS NUM_DOCUMENTO,
                    P.NOMBRES AS NOMBRES,
                    P.APELLIDO_PATERNO AS APELLIDO_PATERNO,
                    P.APELLIDO_MATERNO AS APELLIDO_MATERNO,
                    ISNULL(P.EMAIL, '') AS EMAIL,
                    ISNULL(P.CELULAR, '') AS CELULAR,
                    EP.FECHA_REG AS FECHA_REG
                FROM dbo.EVENTO_PARTICIPANTE EP
                INNER JOIN dbo.PERSONA P ON P.ID_PERSONA = EP.ID_PERSONA
                LEFT JOIN dbo.MAE_TIPO_DOCUMENTO TD ON TD.ID_TIPO_DOCUMENTO = P.ID_TIPO_DOCUMENTO
                WHERE EP.ID_EVENTO = {idEvento}
                ORDER BY EP.FECHA_REG ASC").ToListAsync();
        }

    }
}
