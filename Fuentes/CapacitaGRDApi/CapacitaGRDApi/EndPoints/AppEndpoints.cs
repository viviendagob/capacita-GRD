using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace CapacitaGRDApi.EndPoints
{
    public static class AppEndpoints
    {
        public static RouteGroupBuilder MapApp(this RouteGroupBuilder group)
        {
            group.MapGet("secciones", Listar);
            

            return group;
        }
        static async Task<Ok<List<SeccionDTO>>> Listar(
            IRepositorioSecciones repositorio
            , IMapper mapper)
        {
            var secciones = await repositorio.Listar();
            var seccionDTO = mapper.Map<List<SeccionDTO>>(secciones);            
            return TypedResults.Ok(seccionDTO);
        }
         
    }
}
