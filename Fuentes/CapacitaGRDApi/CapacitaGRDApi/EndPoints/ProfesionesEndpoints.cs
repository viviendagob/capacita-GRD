using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class ProfesionesEndpoints
    {
        public static RouteGroupBuilder MapProfesiones(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar).AllowAnonymous();
            group.MapGet("/{id:int}", Obtener);
            // Anónimo porque el wizard de autorregistro (app y web) permite añadir una profesión
            // que no está en la lista, antes de que la persona tenga sesión iniciada.
            group.MapPost("/", Agregar).AllowAnonymous();
            group.MapPost("/paginado", Paginar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }
         
        static async Task<Ok<List<ProfesionDTO>>> Listar(IRepositorioProfesiones repositorio
            , IMapper mapper)
        {
            var profesiones = await repositorio.Listar();
            var profesionDTO = mapper.Map<List<ProfesionDTO>>(profesiones);
            return TypedResults.Ok(profesionDTO);
        }

        static async Task<Ok<PaginadorDTO<ProfesionDTO>>> Paginar(
            BusquedaGenericaDTO filtro
            , IRepositorioProfesiones repositorio
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
                where = " AND NOMBRE LIKE '%" + dato.Replace("'", "''") + "%' ";
            }

            Filter filter = new()
            {
                Limit = "FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var profesiones = await repositorio.Paginar(filter);
            var profesionDTO = mapper.Map<List<ProfesionDTO>>(profesiones);
         
            var response = new PaginadorDTO<ProfesionDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (profesiones.Any() ? profesiones.FirstOrDefault().REGISTROS : 0),
                recordsTotal = (profesiones.Any() ? profesiones.FirstOrDefault().REGISTROS : 0),
                data = profesionDTO
            };

            return TypedResults.Ok(response);

        }

        static async Task<Results<Ok<ProfesionDTO>, NotFound>> Obtener(IRepositorioProfesiones repositorio
            , int id
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var profesion = new Profesion()
            {
                ID_PROFESION = result.First().ID_PROFESION,
                NOMBRE = result.First().NOMBRE,
            };

            var profesionDTO = mapper.Map<ProfesionDTO>(profesion);
            return TypedResults.Ok(profesionDTO);
        }

        static async Task<Created<ProfesionDTO>> Agregar(CrearProfesionDTO crearProfesionDTO
           , IRepositorioProfesiones repositorio
           , IOutputCacheStore outputCacheStore
           , IMapper mapper
           )
        {
            var profesion = mapper.Map<Profesion>(crearProfesionDTO);
            var id = await repositorio.Agregar(profesion);
            var profesionDTO = mapper.Map<ProfesionDTO>(profesion);
            profesionDTO.ID_PROFESION = id;

            return TypedResults.Created($"/profesiones/{id}", profesionDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearProfesionDTO crearProfesionDTO
            , IRepositorioProfesiones repositorio
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {

            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var profesion = mapper.Map<Profesion>(crearProfesionDTO);
            profesion.ID_PROFESION = id;

            await repositorio.Actualizar(profesion);
            return TypedResults.NoContent();
        }
        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioProfesiones repositorio
            , IOutputCacheStore outputCacheStore)
        {
            var result = await repositorio.Obtener(id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Eliminar(id);
            return TypedResults.NoContent();
        }

    }
}

