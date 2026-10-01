using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class CargosEndpoints
    {
        public static RouteGroupBuilder MapCargos(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar).AllowAnonymous();
            group.MapGet("/{id:int}", Obtener);
            // Anónimo porque el wizard de autorregistro (app y web) permite añadir un cargo
            // que no está en la lista, antes de que la persona tenga sesión iniciada.
            group.MapPost("/", Agregar).AllowAnonymous();
            group.MapPost("/paginado", Paginar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }
        static async Task<Ok<List<CargoDTO>>> Listar(
            IRepositorioCargos repositorio
            , IMapper mapper)
        {
            var cargos = await repositorio.Listar();
            var cargoDTO = mapper.Map<List<CargoDTO>>(cargos);            
            return TypedResults.Ok(cargoDTO);
        }

        static async Task<Ok<PaginadorDTO<CargoDTO>>> Paginar(
            BusquedaGenericaDTO filtro
            , IRepositorioCargos repositorio
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
            var cargoDTO = mapper.Map<List<CargoDTO>>(cargos);

            var response = new PaginadorDTO<CargoDTO>
            {
                draw = paginado.draw,
                recordsFiltered = (cargos.Any() ? cargos.FirstOrDefault().REGISTROS : 0) ,
                recordsTotal = (cargos.Any() ? cargos.FirstOrDefault().REGISTROS : 0),
                data = cargoDTO
            };

            return TypedResults.Ok(response);
        }

        static async Task<Results<Ok<CargoDTO>, NotFound>> Obtener(IRepositorioCargos repositorio
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

            var cargoDTO = mapper.Map<CargoDTO>(result.FirstOrDefault());
            return TypedResults.Ok(cargoDTO);
        }

        static async Task<Created<CargoDTO>> Agregar(
            CrearCargoDTO crearCargoDTO
            , IRepositorioCargos repositorio
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {
            var cargo = mapper.Map<Cargo>(crearCargoDTO);
            var id = await repositorio.Agregar(cargo);
            var cargoDTO = mapper.Map<CargoDTO>(cargo);
            cargoDTO.ID_CARGO = id;

            return TypedResults.Created($"/cargos/{id}", cargoDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearCargoDTO crearCargoDTO
            , IRepositorioCargos repositorio
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

            var cargo = mapper.Map<Cargo>(crearCargoDTO);
            cargo.ID_CARGO = id;

            await repositorio.Actualizar(cargo);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Eliminar
            (
                int id
            , IRepositorioCargos repositorio
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

