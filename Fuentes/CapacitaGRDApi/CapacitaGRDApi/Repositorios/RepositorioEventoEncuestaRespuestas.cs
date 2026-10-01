using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioEventoEncuestaRespuestas : IRepositorioEventoEncuestaRespuestas
    {
        private readonly DbContextClass DbContext;

        public RepositorioEventoEncuestaRespuestas(DbContextClass DbContext)
        {
            this.DbContext = DbContext;
        }

        public async Task AgregarLote(List<EventoEncuestaRespuesta> respuestas)
        {
            foreach (var r in respuestas)
            {
                var parameter = new List<SqlParameter>
                {
                    new("@ID_EVENTO", r.ID_EVENTO),
                    new("@ID_PERSONA", r.ID_PERSONA),
                    new("@ID_ENCUESTA", r.ID_ENCUESTA),
                    new("@ID_PREGUNTA", r.ID_PREGUNTA),
                    new("@RESPUESTA", r.RESPUESTA),
                };
                await DbContext.Database.ExecuteSqlRawAsync(
                    @"INSERT INTO dbo.EVENTO_ENCUESTA_RESPUESTA (ID_EVENTO, ID_PERSONA, ID_ENCUESTA, ID_PREGUNTA, RESPUESTA)
                      VALUES (@ID_EVENTO, @ID_PERSONA, @ID_ENCUESTA, @ID_PREGUNTA, @RESPUESTA)",
                    parameter.ToArray());
            }
        }

        public async Task<bool> Completada(int idEvento, int idPersona)
        {
            var parameter = new List<SqlParameter>
            {
                new("@ID_EVENTO", idEvento),
                new("@ID_PERSONA", idPersona),
            };
            var count = await DbContext.Database.SqlQuery<int>(
                $"SELECT COUNT(*) AS Value FROM dbo.EVENTO_ENCUESTA_RESPUESTA WHERE ID_EVENTO = {idEvento} AND ID_PERSONA = {idPersona}")
                .FirstOrDefaultAsync();
            return count > 0;
        }

        public async Task<EstadisticaEncuestaDTO> Estadisticas(int idEvento)
        {
            var totalAsistentes = await DbContext.Database.SqlQuery<int>(
                $"SELECT COUNT(DISTINCT ID_PERSONA) AS Value FROM dbo.EVENTO_ASISTENCIA WHERE ID_EVENTO = {idEvento}")
                .FirstOrDefaultAsync();

            var totalRespondieron = await DbContext.Database.SqlQuery<int>(
                $"SELECT COUNT(DISTINCT ID_PERSONA) AS Value FROM dbo.EVENTO_ENCUESTA_RESPUESTA WHERE ID_EVENTO = {idEvento}")
                .FirstOrDefaultAsync();

            // Heurística: no hay una escala estructurada de satisfacción en el esquema actual,
            // así que se cuentan como "satisfechos" las respuestas de personas cuyo texto
            // contiene "satisf" (cubre "Satisfecho" y "Muy satisfecho").
            var personasSatisfechas = await DbContext.Database.SqlQuery<int>(
                $@"SELECT COUNT(DISTINCT ID_PERSONA) AS Value FROM dbo.EVENTO_ENCUESTA_RESPUESTA
                   WHERE ID_EVENTO = {idEvento} AND RESPUESTA LIKE '%satisf%'")
                .FirstOrDefaultAsync();

            var filas = await DbContext.Database.SqlQuery<PreguntaAlternativaFila>(
                $@"SELECT EER.ID_PREGUNTA, ER.NOMBRE AS TextoPregunta, EER.RESPUESTA, COUNT(*) AS Cantidad
                   FROM dbo.EVENTO_ENCUESTA_RESPUESTA EER
                   LEFT JOIN dbo.ENCUESTA_RESPUESTA ER ON ER.ID_RESPUESTA = EER.ID_PREGUNTA
                   WHERE EER.ID_EVENTO = {idEvento}
                   GROUP BY EER.ID_PREGUNTA, ER.NOMBRE, EER.RESPUESTA
                   ORDER BY EER.ID_PREGUNTA, EER.RESPUESTA")
                .ToListAsync();

            var preguntas = filas
                .GroupBy(f => f.ID_PREGUNTA)
                .Select(g => new EstadisticaPreguntaDTO
                {
                    ID_PREGUNTA = g.Key,
                    PREGUNTA = g.First().TextoPregunta ?? ("Pregunta #" + g.Key),
                    Alternativas = g.Select(f => new EstadisticaAlternativaDTO
                    {
                        RESPUESTA = f.RESPUESTA,
                        CANTIDAD = f.Cantidad
                    }).ToList()
                })
                .ToList();

            return new EstadisticaEncuestaDTO
            {
                TOTAL_ASISTENTES = totalAsistentes,
                TOTAL_RESPONDIERON = totalRespondieron,
                PORCENTAJE_RESPUESTA = totalAsistentes == 0 ? 0 : Math.Round(totalRespondieron * 100.0 / totalAsistentes, 1),
                PORCENTAJE_SATISFECHOS = totalRespondieron == 0 ? 0 : Math.Round(personasSatisfechas * 100.0 / totalRespondieron, 1),
                Preguntas = preguntas
            };
        }

        private class PreguntaAlternativaFila
        {
            public int ID_PREGUNTA { get; set; }
            public string? TextoPregunta { get; set; }
            public string RESPUESTA { get; set; } = null!;
            public int Cantidad { get; set; }
        }
    }
}
