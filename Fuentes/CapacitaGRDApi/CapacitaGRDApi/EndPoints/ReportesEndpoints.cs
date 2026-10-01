using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Repositorios;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class ReportesEndpoints
    {
        public static RouteGroupBuilder MapReportes(this RouteGroupBuilder group)
        {
            group.MapGet("/participantes/{idEvento:int}", Participantes);
            group.MapGet("/participantes/{idEvento:int}/excel", ParticipantesExcel);

            return group;
        }

        static async Task<Ok<List<ReporteParticipanteDTO>>> Participantes(
            int idEvento
            , IRepositorioReportes repositorio)
        {
            var lista = await repositorio.ReporteParticipantes(idEvento);
            return TypedResults.Ok(lista);
        }

        static async Task<FileContentHttpResult> ParticipantesExcel(
            int idEvento
            , IRepositorioReportes repositorio)
        {
            var participantes = await repositorio.ReporteParticipantes(idEvento);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Participantes");

            string[] encabezados =
            [
                "Num Documento", "Nombres", "Apellido Paterno", "Apellido Materno", "Email", "Celular",
                "Departamento", "Provincia", "Distrito", "Institución", "Cargo", "Modalidad",
                "Sesiones Totales", "Sesiones Obligatorias", "Sesiones Asistidas",
                "Encuesta Respondida", "Apto Constancia", "Estado Constancia", "Código Constancia", "Fecha Inscripción"
            ];
            for (int i = 0; i < encabezados.Length; i++)
            {
                ws.Cell(1, i + 1).Value = encabezados[i];
            }
            ws.Range(1, 1, 1, encabezados.Length).Style.Font.Bold = true;

            var fila = 2;
            foreach (var p in participantes)
            {
                ws.Cell(fila, 1).Value = p.NUM_DOCUMENTO;
                ws.Cell(fila, 2).Value = p.NOMBRES;
                ws.Cell(fila, 3).Value = p.APELLIDO_PATERNO;
                ws.Cell(fila, 4).Value = p.APELLIDO_MATERNO;
                ws.Cell(fila, 5).Value = p.EMAIL;
                ws.Cell(fila, 6).Value = p.CELULAR;
                ws.Cell(fila, 7).Value = p.DEPARTAMENTO;
                ws.Cell(fila, 8).Value = p.PROVINCIA;
                ws.Cell(fila, 9).Value = p.DISTRITO;
                ws.Cell(fila, 10).Value = p.ENTIDAD;
                ws.Cell(fila, 11).Value = p.CARGO;
                ws.Cell(fila, 12).Value = p.MODALIDAD;
                ws.Cell(fila, 13).Value = p.SESIONES_TOTALES;
                ws.Cell(fila, 14).Value = p.SESIONES_OBLIGATORIAS;
                ws.Cell(fila, 15).Value = p.SESIONES_ASISTIDAS;
                ws.Cell(fila, 16).Value = p.ENCUESTA_RESPONDIDA ? "SI" : "NO";
                ws.Cell(fila, 17).Value = p.APTO_CONSTANCIA ? "SI" : "NO";
                ws.Cell(fila, 18).Value = p.ESTADO_CONSTANCIA;
                ws.Cell(fila, 19).Value = p.CODIGO_CONSTANCIA;
                ws.Cell(fila, 20).Value = p.FECHA_INSCRIPCION.ToString("dd/MM/yyyy HH:mm");
                fila++;
            }

            ws.Columns(1, encabezados.Length).AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return TypedResults.Bytes(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reporte_participantes.xlsx");
        }
    }
}
