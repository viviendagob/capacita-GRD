using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.EndPoints
{
    public static class PersonasDataEndpoints
    {
        public static RouteGroupBuilder MapPersonasData(this RouteGroupBuilder group)
        {
            group.MapGet("/{id:int}", Obtener);
            group.MapGet("/areas", ListarAreasLaborales).AllowAnonymous();

            return group;
        }

        static async Task<Ok<List<string>>> ListarAreasLaborales(IRepositorioPersonasData repositorio)
        {
            var areas = await repositorio.ListarAreasLaborales();
            return TypedResults.Ok(areas);
        }

        static async Task<Results<Ok<PersonaData>, NotFound>> Obtener(
            IRepositorioPersonasData repositorio
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

            var personaDataDTO = mapper.Map<PersonaData>(result.FirstOrDefault());            
            return TypedResults.Ok(personaDataDTO);
        }

    }
}
