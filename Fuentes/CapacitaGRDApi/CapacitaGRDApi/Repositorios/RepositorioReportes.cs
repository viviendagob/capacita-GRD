using CapacitaGRDApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioReportes : IRepositorioReportes
    {
        private readonly DbContextClass DbContext;

        public RepositorioReportes(DbContextClass DbContext)
        {
            this.DbContext = DbContext;
        }

        public async Task<List<ReporteParticipanteDTO>> ReporteParticipantes(int idEvento)
        {
            var filas = await DbContext.Database.SqlQuery<ReporteParticipanteDTO>($@"
                SELECT
                    P.ID_PERSONA,
                    P.NUM_DOCUMENTO,
                    P.NOMBRES,
                    P.APELLIDO_PATERNO,
                    P.APELLIDO_MATERNO,
                    P.EMAIL,
                    P.CELULAR,
                    D.DEPARTAMENTO,
                    D.PROVINCIA,
                    D.DISTRITO,
                    COALESCE(EN.NOMBRE, PD.NOMBRE_ENTIDAD_OTRA) AS ENTIDAD,
                    COALESCE(C.NOMBRE, PD.NOMBRE_CARGO_OTRA) AS CARGO,
                    M.NOMBRE AS MODALIDAD,
                    (SELECT COUNT(*) FROM dbo.EVENTO_FECHA WHERE ID_EVENTO = {idEvento}) AS SESIONES_TOTALES,
                    (SELECT COUNT(*) FROM dbo.EVENTO_FECHA WHERE ID_EVENTO = {idEvento} AND OBLIGATORIA = 'S') AS SESIONES_OBLIGATORIAS,
                    (SELECT COUNT(DISTINCT EA.FECHA) FROM dbo.EVENTO_ASISTENCIA EA
                        INNER JOIN dbo.EVENTO_FECHA EF ON EF.ID_EVENTO = EA.ID_EVENTO AND EF.FECHA = EA.FECHA AND EF.OBLIGATORIA = 'S'
                        WHERE EA.ID_EVENTO = {idEvento} AND EA.ID_PERSONA = P.ID_PERSONA) AS SESIONES_ASISTIDAS,
                    CASE WHEN EXISTS(SELECT 1 FROM dbo.EVENTO_ENCUESTA_RESPUESTA WHERE ID_EVENTO = {idEvento} AND ID_PERSONA = P.ID_PERSONA) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS ENCUESTA_RESPONDIDA,
                    CAST(0 AS BIT) AS APTO_CONSTANCIA,
                    COALESCE(EC.ESTADO, 'NO GENERADA') AS ESTADO_CONSTANCIA,
                    EC.CODIGO AS CODIGO_CONSTANCIA,
                    EP.FECHA_REG AS FECHA_INSCRIPCION
                FROM dbo.EVENTO_PARTICIPANTE EP
                INNER JOIN dbo.PERSONA P ON P.ID_PERSONA = EP.ID_PERSONA
                LEFT JOIN dbo.PERSONA_DATA PD ON PD.ID_PERSONA_DATA = EP.ID_PERSONA_DATA
                LEFT JOIN dbo.MAE_ENTIDAD EN ON EN.ID_ENTIDAD = PD.ID_ENTIDAD
                LEFT JOIN dbo.MAE_CARGO C ON C.ID_CARGO = PD.ID_CARGO
                LEFT JOIN dbo.MAE_DISTRITO D ON D.ID_DISTRITO = PD.ID_DISTRITO
                LEFT JOIN dbo.MAE_MODALIDAD M ON M.ID_MODALIDAD = EP.ID_MODALIDAD
                LEFT JOIN dbo.EVENTO_CONSTANCIA EC ON EC.ID_EVENTO = EP.ID_EVENTO AND EC.ID_PERSONA = EP.ID_PERSONA
                WHERE EP.ID_EVENTO = {idEvento}
                ORDER BY P.APELLIDO_PATERNO, P.APELLIDO_MATERNO, P.NOMBRES")
                .ToListAsync();

            foreach (var fila in filas)
            {
                fila.APTO_CONSTANCIA = fila.SESIONES_OBLIGATORIAS == 0
                    ? fila.ENCUESTA_RESPONDIDA
                    : fila.SESIONES_ASISTIDAS >= fila.SESIONES_OBLIGATORIAS && fila.ENCUESTA_RESPONDIDA;
            }

            return filas;
        }
    }
}
