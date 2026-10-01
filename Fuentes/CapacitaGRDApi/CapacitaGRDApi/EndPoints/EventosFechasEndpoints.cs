using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace CapacitaGRDApi.EndPoints
{
    public static class EventosFechasEndpoints
    {
        public static RouteGroupBuilder MapEventosFechas(this RouteGroupBuilder group)
        {
            //group.MapGet("/", Listar);
            group.MapGet("/", Obtener);
            group.MapPost("/", Agregar);
            group.MapPut("/", Actualizar);
            group.MapPut("/actualizar", ActualizarFechas);
            group.MapDelete("/", Eliminar);

            return group;
        }

        static async Task<Results<Ok<EventoFechaDTO>, NotFound>> Obtener(
                IRepositorioEventosFechas repositorio
            ,int id
            , DateTime fecha
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(id, fecha);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var eventoFechaDTO = new EventoFechaDTO()
            {
                ES_CUESTIONARIO = result.FirstOrDefault().ES_CUESTIONARIO,
                ES_ENCUESTA = result.FirstOrDefault().ES_ENCUESTA,  
                FECHA = result.FirstOrDefault().FECHA,
                HORA_FIN = result.FirstOrDefault().HORA_FIN,
                HORA_INICIO = result.FirstOrDefault().HORA_INICIO,
                ID_CUESTIONARIO = result.FirstOrDefault().ID_CUESTIONARIO,
                ID_ENCUESTA = result.FirstOrDefault().ID_ENCUESTA,
                ID_EVENTO = result.FirstOrDefault().ID_EVENTO,
            };

            return TypedResults.Ok(eventoFechaDTO);

        }

        static async Task<Results<NoContent, NotFound>> Agregar(
            CrearEventoFechaDTO crearEventoFechaDTO
           , IRepositorioEventosFechas repositorio
           , IOutputCacheStore outputCacheStore
            , IMapper mapper
           )
        {
            var result = await repositorio.Obtener(crearEventoFechaDTO.ID_EVENTO, crearEventoFechaDTO.FECHA);
            
            if (result.Count() == 0)
            {
                var eventoFecha = mapper.Map<EventoFecha>(crearEventoFechaDTO);
                await repositorio.Agregar(eventoFecha);
                return TypedResults.NoContent();                
            }

            return TypedResults.NotFound();
          
        }

        static async Task<Results<NoContent, NotFound>> ActualizarFechas(
            int id
           , List<CrearEventoFechaDTO> crearEventoFechaDTO
           , IRepositorioEventos repositorioEventos
           , IRepositorioEventosFechas repositorioEventosFechas
           , IOutputCacheStore outputCacheStore
           , IMapper mapper
          )
        {

            var result = await repositorioEventos.Obtener(id);
            if (result is null)
                return TypedResults.NotFound();

            if (!result.Any())
                return TypedResults.NotFound();

            foreach (CrearEventoFechaDTO item in crearEventoFechaDTO)
            {
              
                var eventoFecha = mapper.Map<EventoFecha>(crearEventoFechaDTO.FirstOrDefault());
                eventoFecha.ID_EVENTO = id;

                var existe = await repositorioEventosFechas.Obtener(id, item.FECHA);
                if (existe is not null)
                {
                    if (existe.Any())
                    {
                        await repositorioEventosFechas.Actualizar(eventoFecha);
                    }
                }
            }
        
            return TypedResults.NoContent();
        }


        static async Task<Results<NoContent, NotFound>> Actualizar(
             int id
            , DateTime fecha
            , CrearEventoFechaDTO crearEventoFechaDTO
           , IRepositorioEventosFechas repositorio
           , IOutputCacheStore outputCacheStore
            , IMapper mapper
           )
        {

            var result = await repositorio.Obtener(id, fecha);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var eventoFecha = mapper.Map<EventoFecha>(crearEventoFechaDTO);
            await repositorio.Actualizar(eventoFecha);

            return TypedResults.NoContent();
        }


        static async Task<Results<NoContent, NotFound>> Eliminar(
                int id
            , DateTime fecha
            , IRepositorioEventosFechas repositorio
            , IOutputCacheStore outputCacheStore)
        {
            var result = await repositorio.Obtener(id, fecha);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Eliminar(id, fecha);
            return TypedResults.NoContent();
        }

    }
}
