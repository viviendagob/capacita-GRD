using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class PaisesEndpoints
    {
        public static RouteGroupBuilder MapPaises(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar).AllowAnonymous();
            group.MapGet("/{id:int}", Obtener);
            group.MapPost("/", Agregar);
            group.MapPost("/paginado", Paginar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }

        static async Task<Ok<PaginadorDTO<PaisDTO>>> Paginar(
            BusquedaGenericaDTO filtro
            , IRepositorioPais repositorio
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
                where += " AND NOMBRE LIKE '%" + dato.Replace("'", "''") + "%' ";
            }
             

            Filter filter = new()
            {
                Limit = "FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var paises = await repositorio.Paginar(filter);
            var paisesDTO = mapper.Map<List<PaisDTO>>(paises);

     
            var response = new PaginadorDTO<PaisDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (paises.Any() ? paises.FirstOrDefault().REGISTROS : 0),
                recordsTotal = (paises.Any() ? paises.FirstOrDefault().REGISTROS : 0),
                data = paisesDTO
            };

            return TypedResults.Ok(response);

        }
        static async Task<Ok<List<PaisDTO>>> Listar(IRepositorioPais repositorio
            , IMapper mapper)
        {
            var paises = await repositorio.Listar();
            var paisesDTO = mapper.Map<List<PaisDTO>>(paises);
            return TypedResults.Ok(paisesDTO);
        }

        static async Task<Results<Ok<PaisDTO>, NotFound>> Obtener(IRepositorioPais repositorio
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

            var paisDTO = mapper.Map<PaisDTO>(result.FirstOrDefault());
            return TypedResults.Ok(paisDTO);
        }

        static async Task<Created<PaisDTO>> Agregar(CrearPaisDTO crearPaisDTO
           , IRepositorioPais repositorio
           , IOutputCacheStore outputCacheStore
           , IMapper mapper
           )
        {
            var pais = mapper.Map<Pais>(crearPaisDTO);
            var id = await repositorio.Agregar(pais);
            var paisDTO = mapper.Map<PaisDTO>(pais);
            paisDTO.ID_PAIS = id;

            return TypedResults.Created($"/paises/{id}", paisDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearPaisDTO crearPaisDTO
            , IRepositorioPais repositorio
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

            var pais = mapper.Map<Pais>(crearPaisDTO);
            pais.ID_PAIS = id;

            await repositorio.Actualizar(pais);
            return TypedResults.NoContent();
        }


        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioPais repositorio
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

