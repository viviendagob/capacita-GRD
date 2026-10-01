using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEventosAsistencias : IRepositorioEventosAsistencias
    {
        private readonly DbContextClass DbContext;

        public RepositorioEventosAsistencias(DbContextClass DbContext) {
            this.DbContext = DbContext;
        }
         

        public async Task<IEnumerable<EventoAsistencia>> Existe(EventoAsistencia eventoAsistencia)        
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", eventoAsistencia.ID_EVENTO),
                new("@ID_PERSONA", eventoAsistencia.ID_PERSONA),
                new("@FECHA", eventoAsistencia.FECHA),                
            };

            var result = await Task.Run(() => DbContext.EventoAsistencia.FromSqlRaw(@"exec USP_EVENTO_ASISTENCIA_SEL @ID_EVENTO, @ID_PERSONA, @FECHA ", [..parameter]).AsNoTracking().ToListAsync());            
            return  result;
 
        }

        public async Task<int> Agregar(EventoAsistencia eventoAsistencia)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", eventoAsistencia.ID_EVENTO),
                new("@ID_PERSONA", eventoAsistencia.ID_PERSONA),
                new("@ID_MODALIDAD", eventoAsistencia.ID_MODALIDAD)
            };

            return await Task.Run(() => DbContext.Database.ExecuteSqlRawAsync(@"exec USP_EVENTO_ASISTENCIA_INS @ID_EVENTO , @ID_PERSONA , @ID_MODALIDAD  ", [.. parameter]));
        }

        public async Task<List<EventoAsistencia>> Listar(int idEvento, int idPersona)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", idEvento),
                new("@ID_PERSONA", idPersona)
            };

            return await Task.Run(() => DbContext.EventoAsistencia.FromSqlRaw<EventoAsistencia>(
                @"SELECT *, NULL AS FILA, NULL AS REGISTROS FROM dbo.EVENTO_ASISTENCIA WHERE ID_EVENTO = @ID_EVENTO AND ID_PERSONA = @ID_PERSONA ORDER BY FECHA ASC", [.. parameter]).AsNoTracking().ToListAsync());
        }

        public async Task<List<ReporteAsistenciaDTO>> ListarPorEvento(int idEvento)
        {
            return await DbContext.Database.SqlQuery<ReporteAsistenciaDTO>($@"
                SELECT
                    P.ID_PERSONA AS ID_PERSONA,
                    ISNULL(TD.NOMBRE, '') AS TIPO_DOCUMENTO,
                    P.NUM_DOCUMENTO AS NUM_DOCUMENTO,
                    P.NOMBRES AS NOMBRES,
                    P.APELLIDO_PATERNO AS APELLIDO_PATERNO,
                    P.APELLIDO_MATERNO AS APELLIDO_MATERNO,
                    ISNULL(M.NOMBRE, '') AS MODALIDAD,
                    EA.FECHA_REG AS FECHA_REG
                FROM dbo.EVENTO_ASISTENCIA EA
                INNER JOIN dbo.PERSONA P ON P.ID_PERSONA = EA.ID_PERSONA
                LEFT JOIN dbo.MAE_TIPO_DOCUMENTO TD ON TD.ID_TIPO_DOCUMENTO = P.ID_TIPO_DOCUMENTO
                LEFT JOIN dbo.MAE_MODALIDAD M ON M.ID_MODALIDAD = EA.ID_MODALIDAD
                WHERE EA.ID_EVENTO = {idEvento}
                ORDER BY EA.FECHA_REG ASC").ToListAsync();
        }

    }
}
