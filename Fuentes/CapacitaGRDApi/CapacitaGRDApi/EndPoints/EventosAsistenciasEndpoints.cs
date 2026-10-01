using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace CapacitaGRDApi.EndPoints
{
    public static class EventosAsistenciasEndpoints
    {
        public static RouteGroupBuilder MapEventosAsistencias(this RouteGroupBuilder group)
        {
            group.MapGet("/", Existe);
            group.MapPost("/", Agregar);
            group.MapGet("/evento/{idEvento:int}", ListarPorEvento);
            group.MapGet("/reporte/{idEvento:int}", Reporte);

            return group;
        }

        static async Task<Results<Ok<EventoAsistenciaDTO>, NotFound>> Existe(
            IRepositorioEventosAsistencias repositorio
            , int idPersona
            , int idEvento
            , DateTime fecha
            , IMapper mapper
        )
        {
            var eventoAsistencia = new EventoAsistencia
            {
                ID_EVENTO = idEvento,
                ID_PERSONA = idPersona,
                FECHA = fecha
            };

            var result = await repositorio.Existe(eventoAsistencia);
            if (!result.Any())
            {
                return TypedResults.NotFound();
            }
             
            var eventoAsistenciaDTO = mapper.Map<EventoAsistenciaDTO>(eventoAsistencia);
            return TypedResults.Ok(eventoAsistenciaDTO);
        }


        static async Task<Created<EventoAsistenciaDTO>> Agregar(
              CrearEventoAsistenciaDTO crearEventoAsistenciaDTO
            , IRepositorioEventosAsistencias repositorio
            , IOutputCacheStore outputCacheStore
            , int idPersona
            , int idEvento
            , DateTime fecha
            , IMapper mapper

           )
        {
            var eventoAsistenciaDTO = new EventoAsistenciaDTO();

            var eventoAsistencia = mapper.Map<EventoAsistencia>(crearEventoAsistenciaDTO);
            eventoAsistencia.FECHA = fecha;

            var result = await repositorio.Existe(eventoAsistencia);
            if (!result.Any())
            {
                await repositorio.Agregar(eventoAsistencia);

                eventoAsistenciaDTO = mapper.Map<EventoAsistenciaDTO>(eventoAsistencia);
                return TypedResults.Created("/eventosasistencias", eventoAsistenciaDTO);
            }

            eventoAsistenciaDTO.FECHA = fecha;
            eventoAsistenciaDTO.ID_EVENTO = crearEventoAsistenciaDTO.ID_EVENTO;
            eventoAsistenciaDTO.ID_PERSONA = crearEventoAsistenciaDTO.ID_PERSONA;
            eventoAsistenciaDTO.ID_MODALIDAD = crearEventoAsistenciaDTO.ID_MODALIDAD;

            return TypedResults.Created("/eventosasistencias", eventoAsistenciaDTO);

        }

        static async Task<Ok<List<ReporteAsistenciaDTO>>> ListarPorEvento(
            int idEvento
            , IRepositorioEventosAsistencias repositorio)
        {
            var lista = await repositorio.ListarPorEvento(idEvento);
            return TypedResults.Ok(lista);
        }

        static async Task<FileContentHttpResult> Reporte(
            int idEvento
            , IRepositorioEventosAsistencias repositorio)
        {
            var asistentes = await repositorio.ListarPorEvento(idEvento);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Asistencias");

            ws.Cell(1, 1).Value = "Tipo Documento";
            ws.Cell(1, 2).Value = "Num Documento";
            ws.Cell(1, 3).Value = "Nombres";
            ws.Cell(1, 4).Value = "Apellido Paterno";
            ws.Cell(1, 5).Value = "Apellido Materno";
            ws.Cell(1, 6).Value = "Modalidad";
            ws.Cell(1, 7).Value = "Fecha de Asistencia";
            ws.Range(1, 1, 1, 7).Style.Font.Bold = true;

            var fila = 2;
            foreach (var asistente in asistentes)
            {
                ws.Cell(fila, 1).Value = asistente.TIPO_DOCUMENTO;
                ws.Cell(fila, 2).Value = asistente.NUM_DOCUMENTO;
                ws.Cell(fila, 3).Value = asistente.NOMBRES;
                ws.Cell(fila, 4).Value = asistente.APELLIDO_PATERNO;
                ws.Cell(fila, 5).Value = asistente.APELLIDO_MATERNO;
                ws.Cell(fila, 6).Value = asistente.MODALIDAD;
                ws.Cell(fila, 7).Value = asistente.FECHA_REG.ToString("dd/MM/yyyy HH:mm");
                fila++;
            }

            ws.Columns(1, 7).Width = 18;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return TypedResults.Bytes(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reporte_asistencias.xlsx");
        }

    }
}
