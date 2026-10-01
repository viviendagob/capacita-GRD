using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace CapacitaGRDApi.EndPoints
{
    public static class EncuestaRespuestasEnpoints
    {
        public static RouteGroupBuilder MapEncuestaRespuestas(this RouteGroupBuilder group)
        {
            group.MapGet("/{idEncuesta:int}",  Listar);
            group.MapGet("/{idEncuesta:int}/{id:int}", Obtener);
            group.MapPost("/", Agregar);
            group.MapPut("/", Actualizar);
            group.MapDelete("/", Eliminar);

            return group;
        }
        static async Task<Ok<List<EncuestaRespuestaDTO>>> Listar(
                IRepositorioEncuestaRespuestas repositorio
            , IRepositorioTipoEncuestaRespuestas repositorioTipoEncuestaRespuestas
             , int idEncuesta
            , IMapper mapper)
        {
            var encuestaRespuesta = await repositorio.Listar(idEncuesta);
            var encuestaRespuestaDTO = mapper.Map<List<EncuestaRespuestaDTO>>(encuestaRespuesta);
            foreach (var item in encuestaRespuestaDTO)
            {
                var tipoEncuestaRespuesta = await repositorioTipoEncuestaRespuestas.Obtener(item.ID_TIPO_ENCUESTA_PREGUNTA);
                var tipoEncuestaRespuestaDTO = new TipoEncuestaRespuestaDTO
                {
                    ID_TIPO_ENCUESTA_PREGUNTA = tipoEncuestaRespuesta.FirstOrDefault().ID_TIPO_ENCUESTA_PREGUNTA,
                    NOMBRE = tipoEncuestaRespuesta.FirstOrDefault().NOMBRE
                };

                item.TIPO_ENCUESTA_PREGUNTA = tipoEncuestaRespuestaDTO;
            }


            return TypedResults.Ok(encuestaRespuestaDTO);
        }

        static async Task<Results<Ok<EncuestaRespuestaDTO>, NotFound>> Obtener(
            IRepositorioEncuestaRespuestas repositorio
            , IRepositorioTipoEncuestaRespuestas repositorioTipoEncuestaRespuestas
            , int idEncuesta
            , int id
            , IMapper mapper
            )
        {
            var result = await repositorio.Obtener(idEncuesta, id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }
            
            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var encuestaRespuesta = new EncuestaRespuesta()
            {
                ID_RESPUESTA = result.First().ID_RESPUESTA,
                NOMBRE = result.First().NOMBRE,
                ID_ENCUESTA = result.First().ID_ENCUESTA,   
                ID_TIPO_ENCUESTA_PREGUNTA = result.First().ID_TIPO_ENCUESTA_PREGUNTA,
                RESPUESTAS = result.First().RESPUESTAS                
            };

            var tipoEncuestaRespuesta = await repositorioTipoEncuestaRespuestas.Obtener(encuestaRespuesta.ID_TIPO_ENCUESTA_PREGUNTA);
            var tipoEncuestaRespuestaDTO = new TipoEncuestaRespuestaDTO
            {
                ID_TIPO_ENCUESTA_PREGUNTA = tipoEncuestaRespuesta.FirstOrDefault().ID_TIPO_ENCUESTA_PREGUNTA,
                NOMBRE  = tipoEncuestaRespuesta.FirstOrDefault().NOMBRE
            };


            var encuestaRespuestaDTO = mapper.Map<EncuestaRespuestaDTO>(encuestaRespuesta);
            encuestaRespuestaDTO.TIPO_ENCUESTA_PREGUNTA = tipoEncuestaRespuestaDTO;

            return TypedResults.Ok(encuestaRespuestaDTO);
        }

        static async Task<Created<EncuestaRespuestaDTO>> Agregar(
            CrearEncuestaRespuestaDTO crearEncuestaRespuestaDTO
            , IRepositorioEncuestaRespuestas repositorio
            , IRepositorioTipoEncuestaRespuestas repositorioTipoEncuestaRespuestas
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {
            var encuestaRespuesta = mapper.Map<EncuestaRespuesta>(crearEncuestaRespuestaDTO);
            var id = await repositorio.Agregar(encuestaRespuesta);
            var encuestaRespuestaDTO = mapper.Map<EncuestaRespuestaDTO>(encuestaRespuesta);
           

            var tipoEncuestaRespuesta = await repositorioTipoEncuestaRespuestas.Obtener(encuestaRespuesta.ID_TIPO_ENCUESTA_PREGUNTA);
            var tipoEncuestaRespuestaDTO = new TipoEncuestaRespuestaDTO
            {
                ID_TIPO_ENCUESTA_PREGUNTA = tipoEncuestaRespuesta.FirstOrDefault().ID_TIPO_ENCUESTA_PREGUNTA,
                NOMBRE = tipoEncuestaRespuesta.FirstOrDefault().NOMBRE
            };

            encuestaRespuestaDTO.TIPO_ENCUESTA_PREGUNTA = tipoEncuestaRespuestaDTO;
            encuestaRespuestaDTO.ID_RESPUESTA = id;

            return TypedResults.Created($"/encuestarespuestas/0/{id}", encuestaRespuestaDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id
            , CrearEncuestaRespuestaDTO crearEncuestaRespuestaDTO
            , IRepositorioEncuestaRespuestas repositorio
            , IOutputCacheStore outputCacheStore
            , IMapper mapper
            )
        {

            var result = await repositorio.Obtener(0, id);
            if (result is null)
            {
                return TypedResults.NotFound();
            }

            if (result.Count() == 0)
            {
                return TypedResults.NotFound();
            }

            var encuestaRespuesta = mapper.Map<EncuestaRespuesta>(crearEncuestaRespuestaDTO);
            encuestaRespuesta.ID_RESPUESTA = id;

            await repositorio.Actualizar(encuestaRespuesta);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Eliminar(int id
            , IRepositorioEncuestaRespuestas repositorio
            , IOutputCacheStore outputCacheStore)
        {
            var result = await repositorio.Obtener(0, id);
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
