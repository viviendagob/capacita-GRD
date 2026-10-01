using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class EntidadesEndpoints
    {
        public static RouteGroupBuilder MapEntidades(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar).AllowAnonymous();
            group.MapGet("/{id:int}", Obtener);             
            return group;
        }

        static async Task<Ok<List<EntidadDTO>>> Listar(IRepositorioEntidades repositorio
            , IMapper mapper)
        {
            var entidades = await repositorio.Listar();
            var entidadDTO = mapper.Map<List<EntidadDTO>>(entidades);            
            return TypedResults.Ok(entidadDTO);
        }

        static async Task<Results<Ok<EntidadDTO>, NotFound>> Obtener(IRepositorioEntidades repositorio
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

            var entidad = new Entidad()
            {
                ID_ENTIDAD = result.First().ID_ENTIDAD,
                NOMBRE = result.First().NOMBRE,
            };
             
            var entidadDTO = mapper.Map<EntidadDTO>(entidad);
            return TypedResults.Ok(entidadDTO);
        }
         

    }
}

