using CapacitaGRDApi.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CapacitaGRDApi.Repositorios
{
    public class RepositorioDashboard : IRepositorioDashboard
    {
        private readonly DbContextClass DbContext;

        public RepositorioDashboard(DbContextClass DbContext)
        {
            this.DbContext = DbContext;
        }

        public async Task<DashboardDTO> Kpis(DashboardFiltroDTO filtro)
        {
            // Mismo estilo que el resto del repositorio (WHERE armado en texto con escape
            // manual), consistente con ConsultaParticipantes / RepositorioEventoConstancias.Buscar.
            var where = " WHERE 1=1 ";
            if (filtro.ID_EVENTO.HasValue && filtro.ID_EVENTO.Value > 0)
            {
                where += " AND EP.ID_EVENTO = " + filtro.ID_EVENTO.Value + " ";
            }
            if (filtro.FECHA_DESDE.HasValue)
            {
                where += " AND EP.FECHA_REG >= '" + filtro.FECHA_DESDE.Value.ToString("yyyy-MM-dd") + "' ";
            }
            if (filtro.FECHA_HASTA.HasValue)
            {
                where += " AND EP.FECHA_REG < DATEADD(DAY, 1, '" + filtro.FECHA_HASTA.Value.ToString("yyyy-MM-dd") + "') ";
            }
            if (filtro.ID_MODALIDAD.HasValue && filtro.ID_MODALIDAD.Value > 0)
            {
                where += " AND EP.ID_MODALIDAD = " + filtro.ID_MODALIDAD.Value + " ";
            }
            if (!string.IsNullOrWhiteSpace(filtro.DEPARTAMENTO))
            {
                where += " AND D.DEPARTAMENTO = '" + filtro.DEPARTAMENTO.Replace("'", "''") + "' ";
            }
            if (!string.IsNullOrWhiteSpace(filtro.PROVINCIA))
            {
                where += " AND D.PROVINCIA = '" + filtro.PROVINCIA.Replace("'", "''") + "' ";
            }
            if (!string.IsNullOrWhiteSpace(filtro.DISTRITO))
            {
                where += " AND D.DISTRITO = '" + filtro.DISTRITO.Replace("'", "''") + "' ";
            }
            if (filtro.ID_ENTIDAD.HasValue && filtro.ID_ENTIDAD.Value > 0)
            {
                where += " AND PD.ID_ENTIDAD = " + filtro.ID_ENTIDAD.Value + " ";
            }
            if (filtro.ID_ESTADO.HasValue && filtro.ID_ESTADO.Value > 0)
            {
                where += " AND EV.ID_ESTADO = " + filtro.ID_ESTADO.Value + " ";
            }

            var sql = @"
                SELECT
                    EP.ID_EVENTO,
                    EP.ID_PERSONA,
                    M.NOMBRE AS MODALIDAD,
                    D.DEPARTAMENTO,
                    EN.NOMBRE AS ENTIDAD,
                    ES.NOMBRE AS ESTADO_EVENTO
                FROM dbo.EVENTO_PARTICIPANTE EP
                INNER JOIN dbo.EVENTO EV ON EV.ID_EVENTO = EP.ID_EVENTO
                LEFT JOIN dbo.MAE_ESTADO ES ON ES.ID_ESTADO = EV.ID_ESTADO
                LEFT JOIN dbo.MAE_MODALIDAD M ON M.ID_MODALIDAD = EP.ID_MODALIDAD
                LEFT JOIN dbo.PERSONA_DATA PD ON PD.ID_PERSONA_DATA = EP.ID_PERSONA_DATA
                LEFT JOIN dbo.MAE_DISTRITO D ON D.ID_DISTRITO = PD.ID_DISTRITO
                LEFT JOIN dbo.MAE_ENTIDAD EN ON EN.ID_ENTIDAD = PD.ID_ENTIDAD
                " + where;

            var participantes = await DbContext.Database
                .SqlQuery<ParticipanteFila>(FormattableStringFactory.Create(sql))
                .ToListAsync();

            var dashboard = new DashboardDTO
            {
                TOTAL_EVENTOS = participantes.Select(p => p.ID_EVENTO).Distinct().Count(),
                TOTAL_PARTICIPANTES = participantes.Count,
                TOTAL_INSTITUCIONES = participantes.Where(p => !string.IsNullOrEmpty(p.ENTIDAD)).Select(p => p.ENTIDAD).Distinct().Count(),
                TOTAL_DISTRITOS = participantes.Where(p => !string.IsNullOrEmpty(p.DEPARTAMENTO)).Select(p => p.DEPARTAMENTO).Distinct().Count(),
                POR_MODALIDAD = participantes
                    .GroupBy(p => p.MODALIDAD ?? "Sin modalidad")
                    .Select(g => new KpiSerieDTO { ETIQUETA = g.Key, CANTIDAD = g.Count() })
                    .OrderByDescending(k => k.CANTIDAD)
                    .ToList(),
                POR_DEPARTAMENTO = participantes
                    .Where(p => !string.IsNullOrEmpty(p.DEPARTAMENTO))
                    .GroupBy(p => p.DEPARTAMENTO!)
                    .Select(g => new KpiSerieDTO { ETIQUETA = g.Key, CANTIDAD = g.Count() })
                    .OrderByDescending(k => k.CANTIDAD)
                    .Take(10)
                    .ToList(),
                POR_ESTADO_EVENTO = participantes
                    .GroupBy(p => new { p.ID_EVENTO, Estado = p.ESTADO_EVENTO ?? "Sin estado" })
                    .GroupBy(g => g.Key.Estado)
                    .Select(g => new KpiSerieDTO { ETIQUETA = g.Key, CANTIDAD = g.Count() })
                    .OrderByDescending(k => k.CANTIDAD)
                    .ToList(),
            };

            if (participantes.Count == 0)
            {
                return dashboard;
            }

            var idsEventos = participantes.Select(p => p.ID_EVENTO).Distinct().ToList();
            var pares = participantes.Select(p => (p.ID_EVENTO, p.ID_PERSONA)).ToHashSet();
            var idsEventosCsv = string.Join(",", idsEventos);

            var asistencias = await DbContext.Database.SqlQuery<AsistenciaFila>(
                FormattableStringFactory.Create($"SELECT ID_EVENTO, ID_PERSONA, FECHA FROM dbo.EVENTO_ASISTENCIA WHERE ID_EVENTO IN ({idsEventosCsv})"))
                .ToListAsync();
            var asistenciasFiltradas = asistencias.Where(a => pares.Contains((a.ID_EVENTO, a.ID_PERSONA))).ToList();

            var sesionesObligatorias = await DbContext.Database.SqlQuery<SesionFila>(
                FormattableStringFactory.Create($"SELECT ID_EVENTO, FECHA FROM dbo.EVENTO_FECHA WHERE ID_EVENTO IN ({idsEventosCsv}) AND OBLIGATORIA = 'S'"))
                .ToListAsync();
            var obligatoriasPorEvento = sesionesObligatorias.GroupBy(s => s.ID_EVENTO).ToDictionary(g => g.Key, g => g.Select(s => s.FECHA).ToHashSet());

            var encuestas = await DbContext.Database.SqlQuery<EncuestaFila>(
                FormattableStringFactory.Create($"SELECT DISTINCT ID_EVENTO, ID_PERSONA FROM dbo.EVENTO_ENCUESTA_RESPUESTA WHERE ID_EVENTO IN ({idsEventosCsv})"))
                .ToListAsync();
            var encuestasSet = encuestas.Select(e => (e.ID_EVENTO, e.ID_PERSONA)).ToHashSet();

            var satisfechos = await DbContext.Database.SqlQuery<EncuestaFila>(
                FormattableStringFactory.Create($@"SELECT DISTINCT ID_EVENTO, ID_PERSONA FROM dbo.EVENTO_ENCUESTA_RESPUESTA
                    WHERE ID_EVENTO IN ({idsEventosCsv}) AND RESPUESTA LIKE '%satisf%'"))
                .ToListAsync();
            var satisfechosSet = satisfechos.Select(e => (e.ID_EVENTO, e.ID_PERSONA)).ToHashSet();

            var constancias = await DbContext.Database.SqlQuery<ConstanciaFila>(
                FormattableStringFactory.Create($"SELECT ID_EVENTO, ID_PERSONA, ESTADO FROM dbo.EVENTO_CONSTANCIA WHERE ID_EVENTO IN ({idsEventosCsv})"))
                .ToListAsync();
            var constanciasFiltradas = constancias.Where(c => pares.Contains((c.ID_EVENTO, c.ID_PERSONA))).ToList();

            var asistentesUnicos = asistenciasFiltradas.Select(a => (a.ID_EVENTO, a.ID_PERSONA)).Distinct().Count();
            var sesionesAsistidasPorPar = asistenciasFiltradas
                .GroupBy(a => (a.ID_EVENTO, a.ID_PERSONA))
                .ToDictionary(g => g.Key, g => g.Select(a => a.FECHA).Distinct()
                    .Count(f => obligatoriasPorEvento.TryGetValue(g.Key.Item1, out var set) && set.Contains(f)));

            var aptos = 0;
            foreach (var par in pares)
            {
                var obligatoriasCount = obligatoriasPorEvento.TryGetValue(par.Item1, out var set) ? set.Count : 0;
                var asistidas = sesionesAsistidasPorPar.TryGetValue(par, out var c) ? c : 0;
                var encuestaOk = encuestasSet.Contains(par);
                var apto = obligatoriasCount == 0 ? encuestaOk : (asistidas >= obligatoriasCount && encuestaOk);
                if (apto) aptos++;
            }

            dashboard.TOTAL_ASISTENTES = asistentesUnicos;
            dashboard.PCT_ASISTENCIA = dashboard.TOTAL_PARTICIPANTES == 0 ? 0 : Math.Round(asistentesUnicos * 100.0 / dashboard.TOTAL_PARTICIPANTES, 1);
            dashboard.TOTAL_CONSTANCIAS_GENERADAS = constanciasFiltradas.Count(c => c.ESTADO == "GENERADA");
            dashboard.TOTAL_CONSTANCIAS_ANULADAS = constanciasFiltradas.Count(c => c.ESTADO == "ANULADA");
            dashboard.PCT_APTO_CONSTANCIA = dashboard.TOTAL_PARTICIPANTES == 0 ? 0 : Math.Round(aptos * 100.0 / dashboard.TOTAL_PARTICIPANTES, 1);
            dashboard.TOTAL_RESPONDIERON_ENCUESTA = encuestasSet.Count(e => pares.Contains(e));
            dashboard.PCT_RESPUESTA_ENCUESTA = asistentesUnicos == 0 ? 0 : Math.Round(dashboard.TOTAL_RESPONDIERON_ENCUESTA * 100.0 / asistentesUnicos, 1);
            var satisfechosFiltrados = satisfechosSet.Count(e => pares.Contains(e));
            dashboard.PCT_SATISFACCION = dashboard.TOTAL_RESPONDIERON_ENCUESTA == 0 ? 0 : Math.Round(satisfechosFiltrados * 100.0 / dashboard.TOTAL_RESPONDIERON_ENCUESTA, 1);

            return dashboard;
        }

        private class ParticipanteFila
        {
            public int ID_EVENTO { get; set; }
            public int ID_PERSONA { get; set; }
            public string? MODALIDAD { get; set; }
            public string? DEPARTAMENTO { get; set; }
            public string? ENTIDAD { get; set; }
            public string? ESTADO_EVENTO { get; set; }
        }

        private class AsistenciaFila
        {
            public int ID_EVENTO { get; set; }
            public int ID_PERSONA { get; set; }
            public DateTime FECHA { get; set; }
        }

        private class SesionFila
        {
            public int ID_EVENTO { get; set; }
            public DateTime FECHA { get; set; }
        }

        private class EncuestaFila
        {
            public int ID_EVENTO { get; set; }
            public int ID_PERSONA { get; set; }
        }

        private class ConstanciaFila
        {
            public int ID_EVENTO { get; set; }
            public int ID_PERSONA { get; set; }
            public string ESTADO { get; set; } = null!;
        }
    }
}
