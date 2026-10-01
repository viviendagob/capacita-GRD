using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEventoConstancias : IRepositorioEventoConstancias
    {
        private readonly DbContextClass DbContext;

        public RepositorioEventoConstancias(DbContextClass DbContext)
        {
            this.DbContext = DbContext;
        }

        public async Task<List<ParticipanteConstanciaDTO>> ListarElegibles(int idEvento)
        {
            var filas = await DbContext.Database.SqlQuery<ParticipanteElegibleFila>($@"
                SELECT
                    P.ID_PERSONA,
                    P.NUM_DOCUMENTO,
                    P.NOMBRES,
                    P.APELLIDO_PATERNO,
                    P.APELLIDO_MATERNO,
                    (SELECT COUNT(*) FROM dbo.EVENTO_FECHA WHERE ID_EVENTO = {idEvento} AND OBLIGATORIA = 'S') AS SESIONES_OBLIGATORIAS,
                    (SELECT COUNT(DISTINCT EA.FECHA) FROM dbo.EVENTO_ASISTENCIA EA
                        INNER JOIN dbo.EVENTO_FECHA EF ON EF.ID_EVENTO = EA.ID_EVENTO AND EF.FECHA = EA.FECHA AND EF.OBLIGATORIA = 'S'
                        WHERE EA.ID_EVENTO = {idEvento} AND EA.ID_PERSONA = P.ID_PERSONA) AS SESIONES_ASISTIDAS,
                    CASE WHEN EXISTS(SELECT 1 FROM dbo.EVENTO_ENCUESTA_RESPUESTA WHERE ID_EVENTO = {idEvento} AND ID_PERSONA = P.ID_PERSONA) THEN 1 ELSE 0 END AS ENCUESTA_RESPONDIDA
                FROM dbo.EVENTO_PARTICIPANTE EP
                INNER JOIN dbo.PERSONA P ON P.ID_PERSONA = EP.ID_PERSONA
                WHERE EP.ID_EVENTO = {idEvento}
                ORDER BY P.APELLIDO_PATERNO, P.APELLIDO_MATERNO, P.NOMBRES")
                .ToListAsync();

            var constancias = await DbContext.Database.SqlQuery<ConstanciaFila>($@"
                SELECT ID_CONSTANCIA, ID_EVENTO, ID_PERSONA, CODIGO, ESTADO, URL_PDF, FECHA_GENERACION,
                       USER_GENERACION, FECHA_ULTIMA_DESCARGA, NUM_DESCARGAS, USER_ANULACION, FECHA_ANULACION, MOTIVO_ANULACION
                FROM dbo.EVENTO_CONSTANCIA WHERE ID_EVENTO = {idEvento}")
                .ToListAsync();

            return filas.Select(f =>
            {
                var apto = f.SESIONES_OBLIGATORIAS == 0
                    ? f.ENCUESTA_RESPONDIDA == 1
                    : f.SESIONES_ASISTIDAS >= f.SESIONES_OBLIGATORIAS && f.ENCUESTA_RESPONDIDA == 1;

                var constancia = constancias.FirstOrDefault(c => c.ID_PERSONA == f.ID_PERSONA);

                return new ParticipanteConstanciaDTO
                {
                    ID_PERSONA = f.ID_PERSONA,
                    NUM_DOCUMENTO = f.NUM_DOCUMENTO,
                    NOMBRES = f.NOMBRES,
                    APELLIDO_PATERNO = f.APELLIDO_PATERNO,
                    APELLIDO_MATERNO = f.APELLIDO_MATERNO,
                    SESIONES_OBLIGATORIAS = f.SESIONES_OBLIGATORIAS,
                    SESIONES_ASISTIDAS = f.SESIONES_ASISTIDAS,
                    ENCUESTA_RESPONDIDA = f.ENCUESTA_RESPONDIDA == 1,
                    APTO = apto,
                    Constancia = constancia is null ? null : MapDTO(constancia),
                };
            }).ToList();
        }

        public async Task<EventoConstancia?> Obtener(int idConstancia)
        {
            var result = await DbContext.EventoConstancia
                .FromSqlInterpolated($"SELECT * FROM dbo.EVENTO_CONSTANCIA WHERE ID_CONSTANCIA = {idConstancia}")
                .AsNoTracking().ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<EventoConstancia?> ObtenerPorEventoPersona(int idEvento, int idPersona)
        {
            var result = await DbContext.EventoConstancia
                .FromSqlInterpolated($"SELECT * FROM dbo.EVENTO_CONSTANCIA WHERE ID_EVENTO = {idEvento} AND ID_PERSONA = {idPersona}")
                .AsNoTracking().ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<EventoConstancia?> ObtenerPorCodigo(string codigo)
        {
            var result = await DbContext.EventoConstancia
                .FromSqlInterpolated($"SELECT * FROM dbo.EVENTO_CONSTANCIA WHERE CODIGO = {codigo}")
                .AsNoTracking().ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<List<ConstanciaDTO>> Buscar(string? documento, int? idEvento, string? codigo)
        {
            // Mismo estilo que el resto del repositorio de búsquedas del proyecto (WHERE armado
            // en texto con escape manual de comillas), en vez de SqlQuery<T> interpolado, porque
            // esa API no soporta bien filtros opcionales con cantidad variable de parámetros.
            var where = " WHERE 1=1 ";
            if (!string.IsNullOrWhiteSpace(documento))
            {
                where += " AND P.NUM_DOCUMENTO LIKE '%" + documento.Replace("'", "''") + "%' ";
            }
            if (idEvento.HasValue && idEvento.Value > 0)
            {
                where += " AND C.ID_EVENTO = " + idEvento.Value + " ";
            }
            if (!string.IsNullOrWhiteSpace(codigo))
            {
                where += " AND C.CODIGO LIKE '%" + codigo.Replace("'", "''") + "%' ";
            }

            var sql = @"
                SELECT C.ID_CONSTANCIA, C.ID_EVENTO, C.ID_PERSONA, C.CODIGO, C.ESTADO, C.URL_PDF, C.FECHA_GENERACION,
                       C.USER_GENERACION, C.FECHA_ULTIMA_DESCARGA, C.NUM_DESCARGAS, C.USER_ANULACION, C.FECHA_ANULACION, C.MOTIVO_ANULACION
                FROM dbo.EVENTO_CONSTANCIA C
                INNER JOIN dbo.PERSONA P ON P.ID_PERSONA = C.ID_PERSONA
                " + where + @"
                ORDER BY C.FECHA_GENERACION DESC";

            var filas = await DbContext.Database.SqlQuery<ConstanciaFila>(FormattableStringFactory.Create(sql)).ToListAsync();

            return filas.Select(MapDTO).ToList();
        }

        public async Task<int> Agregar(EventoConstancia constancia)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", constancia.ID_EVENTO),
                new("@ID_PERSONA", constancia.ID_PERSONA),
                new("@CODIGO", constancia.CODIGO),
                new("@URL_PDF", (object?)constancia.URL_PDF ?? DBNull.Value),
                new("@USER_GENERACION", (object?)constancia.USER_GENERACION ?? DBNull.Value),
            };

            await DbContext.Database.ExecuteSqlRawAsync(
                @"INSERT INTO dbo.EVENTO_CONSTANCIA (ID_EVENTO, ID_PERSONA, CODIGO, ESTADO, URL_PDF, USER_GENERACION)
                  VALUES (@ID_EVENTO, @ID_PERSONA, @CODIGO, 'GENERADA', @URL_PDF, @USER_GENERACION)",
                parameter.ToArray());

            var id = await DbContext.Database.SqlQuery<int>(
                $"SELECT ID_CONSTANCIA AS Value FROM dbo.EVENTO_CONSTANCIA WHERE CODIGO = {constancia.CODIGO}")
                .FirstOrDefaultAsync();
            return id;
        }

        public async Task RegistrarDescarga(int idConstancia)
        {
            await DbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE dbo.EVENTO_CONSTANCIA
                SET FECHA_ULTIMA_DESCARGA = GETDATE(), NUM_DESCARGAS = NUM_DESCARGAS + 1
                WHERE ID_CONSTANCIA = {idConstancia}");
        }

        public async Task Anular(int idConstancia, string motivo, string usuario)
        {
            await DbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE dbo.EVENTO_CONSTANCIA
                SET ESTADO = 'ANULADA', MOTIVO_ANULACION = {motivo}, USER_ANULACION = {usuario}, FECHA_ANULACION = GETDATE()
                WHERE ID_CONSTANCIA = {idConstancia}");
        }

        private static ConstanciaDTO MapDTO(ConstanciaFila f) => new()
        {
            ID_CONSTANCIA = f.ID_CONSTANCIA,
            ID_EVENTO = f.ID_EVENTO,
            ID_PERSONA = f.ID_PERSONA,
            CODIGO = f.CODIGO,
            ESTADO = f.ESTADO,
            URL_PDF = f.URL_PDF,
            FECHA_GENERACION = f.FECHA_GENERACION,
            USER_GENERACION = f.USER_GENERACION,
            FECHA_ULTIMA_DESCARGA = f.FECHA_ULTIMA_DESCARGA,
            NUM_DESCARGAS = f.NUM_DESCARGAS,
            USER_ANULACION = f.USER_ANULACION,
            FECHA_ANULACION = f.FECHA_ANULACION,
            MOTIVO_ANULACION = f.MOTIVO_ANULACION,
        };

        private class ParticipanteElegibleFila
        {
            public int ID_PERSONA { get; set; }
            public string NUM_DOCUMENTO { get; set; } = null!;
            public string NOMBRES { get; set; } = null!;
            public string APELLIDO_PATERNO { get; set; } = null!;
            public string APELLIDO_MATERNO { get; set; } = null!;
            public int SESIONES_OBLIGATORIAS { get; set; }
            public int SESIONES_ASISTIDAS { get; set; }
            public int ENCUESTA_RESPONDIDA { get; set; }
        }

        private class ConstanciaFila
        {
            public int ID_CONSTANCIA { get; set; }
            public int ID_EVENTO { get; set; }
            public int ID_PERSONA { get; set; }
            public string CODIGO { get; set; } = null!;
            public string ESTADO { get; set; } = null!;
            public string? URL_PDF { get; set; }
            public DateTime FECHA_GENERACION { get; set; }
            public string? USER_GENERACION { get; set; }
            public DateTime? FECHA_ULTIMA_DESCARGA { get; set; }
            public int NUM_DESCARGAS { get; set; }
            public string? USER_ANULACION { get; set; }
            public DateTime? FECHA_ANULACION { get; set; }
            public string? MOTIVO_ANULACION { get; set; }
        }
    }
}
