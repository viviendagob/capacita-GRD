using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class EstadosEnpoints
    {
        public static RouteGroupBuilder MapEstados(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
           
            return group;
        }

        static async Task<Ok<List<EstadoDTO>>> Listar(
                IRepositorioEstados repositorio
            , IMapper mapper)
        {
            var estados = await repositorio.Listar();
            var estadosDTO = mapper.Map<List<EstadoDTO>>(estados);
            return TypedResults.Ok(estadosDTO);
        }

        static async Task<Results<Ok<EstadoDTO>, NotFound>> Obtener(
                IRepositorioEstados repositorio
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

            var estadoDTO = mapper.Map<EstadoDTO>(result.FirstOrDefault());
            return TypedResults.Ok(estadoDTO);
        }


    }
}
