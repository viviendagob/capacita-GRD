using AutoMapper;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using CapacitaGRDApi.Util;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace CapacitaGRDApi.EndPoints
{
    public static class ConfiguracionEndpoints
    {
        public static RouteGroupBuilder MapConfiguracion(this RouteGroupBuilder group)
        {
            group.MapGet("/", Obtener);
            group.MapPut("/", Actualizar);


            return group;
        }
         
        static async Task<Results<Ok<ConfiguracionDTO>, NotFound>> Obtener(
             IRepositorioConfiguracion repositorio
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(Constantes.idConfiguracion);
            if (result is null)
                return TypedResults.NotFound();

            if (!result.Any())
                return TypedResults.NotFound();

            var configuracion = result.FirstOrDefault();
            var configuracionDTO = mapper.Map<ConfiguracionDTO>(configuracion);

            return TypedResults.Ok(configuracionDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(
             CrearConfiguracionDTO crearConfiguracionDTO
           , IRepositorioConfiguracion repositorio
           , IOutputCacheStore outputCacheStore
           , IMapper mapper
           )
        {

            var result = await repositorio.Obtener(Constantes.idConfiguracion);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var configuracion = mapper.Map<Configuracion>(crearConfiguracionDTO);
            configuracion.ID_CONFIGURACION = Constantes.idConfiguracion;

            await repositorio.Actualizar(configuracion);
            return TypedResults.NoContent();
        }



    }
}
