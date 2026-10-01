using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class DocumentosEndpoints
    {
        public static RouteGroupBuilder MapDocumentosRequeridos(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
            group.MapPost("/", Agregar);
            group.MapPost("/paginado", Paginar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }
        static async Task<Ok<List<DocumentoDTO>>> Listar(
            IRepositorioDocumentos repositorio
            , IMapper mapper)
        {
            var documentos = await repositorio.Listar();
            var documentosDTO = mapper.Map<List<DocumentoDTO>>(documentos);            
            return TypedResults.Ok(documentosDTO);
        }

        static async Task<Ok<PaginadorDTO<DocumentoDTO>>> Paginar(
            BusquedaGenericaDTO filtro
            , IRepositorioDocumentos repositorio
            , IMapper mapper)
            {

            var paginado = filtro.Filtro;
            var dato = filtro.Dato;

            int pagActual = paginado.start;
            int pagDesde = (pagActual == 0) ? 1 : (pagActual + 1);
            int pagHasta = pagActual + paginado.length;
            var inxColumn = paginado.order[0].column;
            var orderColumn = paginado.order[0].dir;
            var orderBy = paginado.columns[inxColumn].name + ' ' + orderColumn;

            var where = " ";
            if (dato.Trim() != "")
            {
                where = " AND NOMBRE LIKE '%"  + dato.Replace("'", "''") + "%' ";
            }

            Filter filter = new()
            {
                Limit = "FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var cargos = await repositorio.Paginar(filter);
            var documentoDTO = mapper.Map<List<DocumentoDTO>>(cargos);

            var response = new PaginadorDTO<DocumentoDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (cargos.Any() ? cargos.FirstOrDefault().REGISTROS : 0) ,
                recordsTotal = (cargos.Any() ? cargos.FirstOrDefault().REGISTROS : 0),
                data = documentoDTO
            };

            return TypedResults.Ok(response);
        }

        static async Task<Results<Ok<DocumentoDTO>, NotFound>> Obtener(
            IRepositorioDocumentos repositorio
            , int id
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }
            
            if (!result.Any())
            {
                return TypedResults.NotFound();
            }
 
             
            var documentoDTO = mapper.Map<DocumentoDTO>(result.FirstOrDefault());
            return TypedResults.Ok(documentoDTO);
        }

        static async Task<Created<DocumentoDTO>> Agregar(
            CrearDocumentoDTO crearDocumentoDTO
            , IRepositorioDocumentos repositorio
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {
            var documento = mapper.Map<Documento>(crearDocumentoDTO);
            var id = await repositorio.Agregar(documento);
            var documentoDTO = mapper.Map<DocumentoDTO>(documento);
            documentoDTO.ID_TIPO_DOCUMENTO_REQUERIDO = id;

            return TypedResults.Created($"/documentosrequeridos/{id}", documentoDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearDocumentoDTO crearDocumentoDTO
            , IRepositorioDocumentos repositorio
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {

            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (!result.Any())
            {
                return TypedResults.NotFound();
            }

            var documento = mapper.Map<Documento>(crearDocumentoDTO);
            documento.ID_TIPO_DOCUMENTO_REQUERIDO = id;

            await repositorio.Actualizar(documento);
            return TypedResults.NoContent();
        }
        static async Task<Results<NoContent, NotFound>> Eliminar(
            int id
            , IRepositorioDocumentos repositorio
            , IOutputCacheStore outputCacheStore)
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (!result.Any())
            {
                return TypedResults.NotFound();
            }

            await repositorio.Eliminar(id);
            return TypedResults.NoContent();
        }

    }
}
