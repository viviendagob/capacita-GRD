using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using CapacitaGRDApi.Util;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class DistritosEndpoints
    {
        public static RouteGroupBuilder MapDistritos(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar).AllowAnonymous();
            group.MapGet("/{id:int}", Obtener);
            group.MapPost("/", Agregar);
            group.MapPost("/paginado", Paginar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }
         
        static async Task<Ok<List<DistritoDTO>>> Listar(IRepositorioDistritos repositorio
            , IMapper mapper)
        {
            var distritos = await repositorio.Listar();
            var distritosDTO = mapper.Map<List<DistritoDTO>>(distritos);
            return TypedResults.Ok(distritosDTO);
        }

        static async Task<Ok<PaginadorDTO<DistritoDTO>>> Paginar(            
            BusquedaDistritoDTO filtro
            , IRepositorioDistritos repositorio
            , IMapper mapper)
        {
            var paginado = filtro.Filtro;
            var dato = filtro.Dato;
            var pais = filtro.IdPais;

            int pagActual = paginado.start;
            int pagDesde = (pagActual == 0) ? 1 : (pagActual + 1);
            int pagHasta = pagActual + paginado.length;
            var inxColumn = paginado.order[0].column;
            var orderColumn = paginado.order[0].dir;
            var orderBy = paginado.columns[inxColumn].name + ' ' + orderColumn;

            var where = " ";
            if (dato.Trim() != "")
            {
                where += " AND DISTRITO LIKE '%" + dato.Replace("'", "''") + "%' ";
            }

            if (pais != 0)
            {
                where += " AND ID_PAIS = " + pais;
            }


            Filter filter = new()
            {
                Limit = "FILA BETWEEN " + pagDesde + " AND " + pagHasta,
                Order = orderBy,
                Where = where
            };

            var distritos = await repositorio.Paginar(filter);
            var distritosDTO = mapper.Map<List<DistritoDTO>>(distritos);
 
            var response = new PaginadorDTO<DistritoDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (distritos.Any() ? distritos.FirstOrDefault().REGISTROS : 0),
                recordsTotal = (distritos.Any() ? distritos.FirstOrDefault().REGISTROS : 0),
                data = distritosDTO
            };

            return TypedResults.Ok(response);

        }

        static async Task<Results<Ok<DistritoDTO>, NotFound>> Obtener(
            IRepositorioDistritos repositorio
            , IRepositorioPais repositorioPais
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

            

            var distrito = new Distrito()
            {
                ID_DISTRITO = result.First().ID_DISTRITO, 
                CODIGO_DEPARTAMENTO = result.First().CODIGO_DEPARTAMENTO,
                DEPARTAMENTO = result.First().DEPARTAMENTO,
                CODIGO_PROVINCIA = result.First().PROVINCIA,
                PROVINCIA = result.First().PROVINCIA,
                CODIGO_DISTRITO = result.First().CODIGO_DISTRITO,
                DISTRITO = result.First().DISTRITO,
                CAPITAL = result.First().CAPITAL,
                CATEGORIA = result.First().CATEGORIA,
                ID_PAIS = result.First().ID_PAIS,                
            };

            var pais = await repositorioPais.Obtener(distrito.ID_PAIS);

            var distritoDTO = mapper.Map<DistritoDTO>(distrito);
            distritoDTO.PAIS = pais;

            return TypedResults.Ok(distritoDTO);
        }

        static async Task<Created<DistritoDTO>> Agregar(CrearDistritoDTO crearDistritoDTO
           , IRepositorioDistritos repositorio
           , IOutputCacheStore outputCacheStore
           , IMapper mapper
           )
        {
            var distrito = mapper.Map<Distrito>(crearDistritoDTO);
            var id = await repositorio.Agregar(distrito);
            var distritoDTO = mapper.Map<DistritoDTO>(distrito);
            distritoDTO.ID_DISTRITO = id;

            return TypedResults.Created($"/distritos/{id}", distritoDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearDistritoDTO crearDistritoDTO
            , IRepositorioDistritos repositorio
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

            var distrito = mapper.Map<Distrito>(crearDistritoDTO);
            distrito.ID_DISTRITO = id;

            await repositorio.Actualizar(distrito);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioDistritos repositorio
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

