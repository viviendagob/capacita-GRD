using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    // Registro de las respuestas REALES de cada participante (no confundir con
    // /encuestarespuestas, que es la lista maestra de alternativas posibles).
    public static class EventoEncuestaRespuestasEndpoints
    {
        public static RouteGroupBuilder MapEventoEncuestaRespuestas(this RouteGroupBuilder group)
        {
            group.MapPost("/responder", Responder);
            group.MapGet("/completada", Completada);
            group.MapGet("/estadisticas/{idEvento:int}", Estadisticas);
            group.MapGet("/estadisticas/{idEvento:int}/excel", EstadisticasExcel);

            return group;
        }

        static async Task<Ok> Responder(
            List<CrearEventoEncuestaRespuestaDTO> respuestas
            , IRepositorioEventoEncuestaRespuestas repositorio
        )
        {
            var entidades = respuestas.Select(r => new EventoEncuestaRespuesta
            {
                ID_EVENTO = r.ID_EVENTO,
                ID_PERSONA = r.ID_PERSONA,
                ID_ENCUESTA = r.ID_ENCUESTA,
                ID_PREGUNTA = r.ID_PREGUNTA,
                RESPUESTA = r.RESPUESTA,
            }).ToList();

            await repositorio.AgregarLote(entidades);
            return TypedResults.Ok();
        }

        static async Task<Ok<bool>> Completada(
            int idEvento
            , int idPersona
            , IRepositorioEventoEncuestaRespuestas repositorio
        )
        {
            var completada = await repositorio.Completada(idEvento, idPersona);
            return TypedResults.Ok(completada);
        }

        static async Task<Ok<EstadisticaEncuestaDTO>> Estadisticas(
            int idEvento
            , IRepositorioEventoEncuestaRespuestas repositorio
        )
        {
            var estadisticas = await repositorio.Estadisticas(idEvento);
            return TypedResults.Ok(estadisticas);
        }

        static async Task<FileContentHttpResult> EstadisticasExcel(
            int idEvento
            , IRepositorioEventoEncuestaRespuestas repositorio
        )
        {
            var est = await repositorio.Estadisticas(idEvento);

            using var workbook = new XLWorkbook();
            var resumen = workbook.Worksheets.Add("Resumen");
            resumen.Cell(1, 1).Value = "Total asistentes";
            resumen.Cell(1, 2).Value = est.TOTAL_ASISTENTES;
            resumen.Cell(2, 1).Value = "Total respondieron";
            resumen.Cell(2, 2).Value = est.TOTAL_RESPONDIERON;
            resumen.Cell(3, 1).Value = "% Respuesta";
            resumen.Cell(3, 2).Value = est.PORCENTAJE_RESPUESTA;
            resumen.Cell(4, 1).Value = "Meta de respuesta (%)";
            resumen.Cell(4, 2).Value = est.META_RESPUESTA;
            resumen.Cell(5, 1).Value = "% Satisfechos";
            resumen.Cell(5, 2).Value = est.PORCENTAJE_SATISFECHOS;
            resumen.Column(1).Style.Font.Bold = true;
            resumen.Columns(1, 2).AdjustToContents();

            var detalle = workbook.Worksheets.Add("Por Pregunta");
            detalle.Cell(1, 1).Value = "Pregunta";
            detalle.Cell(1, 2).Value = "Alternativa";
            detalle.Cell(1, 3).Value = "Cantidad";
            detalle.Range(1, 1, 1, 3).Style.Font.Bold = true;

            var fila = 2;
            foreach (var pregunta in est.Preguntas)
            {
                foreach (var alt in pregunta.Alternativas)
                {
                    detalle.Cell(fila, 1).Value = pregunta.PREGUNTA;
                    detalle.Cell(fila, 2).Value = alt.RESPUESTA;
                    detalle.Cell(fila, 3).Value = alt.CANTIDAD;
                    fila++;
                }
            }
            detalle.Columns(1, 3).AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return TypedResults.Bytes(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "estadisticas_encuesta.xlsx");
        }
    }
}
