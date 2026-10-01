using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace CapacitaGRDApi.EndPoints
{
    public static class SeccionesEndpoints
    {
        public static RouteGroupBuilder MapSecciones(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
            group.MapPost("/", Agregar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }
        static async Task<Ok<List<SeccionDTO>>> Listar(IRepositorioSecciones repositorio
            , IMapper mapper)
        {
            var secciones = await repositorio.Listar();
            var seccionesDTO = mapper.Map<List<SeccionDTO>>(secciones);            
            return TypedResults.Ok(seccionesDTO);
        }

        static async Task<Results<Ok<SeccionDTO>, NotFound>> Obtener(
            IRepositorioSecciones repositorio
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

            var seccion = new Seccion()
            {
                ID_SECCION = result.First().ID_SECCION,
                NOMBRE = result.First().NOMBRE,
            };

            var seccionDTO = mapper.Map<SeccionDTO>(seccion);
            return TypedResults.Ok(seccionDTO);
        }

        static async Task<Created<SeccionDTO>> Agregar(
                CrearSeccionDTO crearSeccionDTO
            , IRepositorioSecciones repositorio
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {
            var seccion = mapper.Map<Seccion>(crearSeccionDTO);
            var id = await repositorio.Agregar(seccion);
            var seccionDTO = mapper.Map<SeccionDTO>(seccion);
            seccionDTO.ID_SECCION = id;

            return TypedResults.Created($"/secciones/{id}", seccionDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearSeccionDTO crearSeccionDTO
            , IRepositorioSecciones repositorio
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

            var seccion = mapper.Map<Seccion>(crearSeccionDTO);
            seccion.ID_SECCION = id;

            await repositorio.Actualizar(seccion);
            return TypedResults.NoContent();
        }


        static async Task<Results<NoContent, NotFound>> Eliminar(
            int id
            , IRepositorioSecciones repositorio
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
