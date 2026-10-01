using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class TipoEncuestaRespuestasEndPoint
    {
        public static RouteGroupBuilder MapTipoEncuestaRespuestas(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
           
            return group;
        }

        static async Task<Ok<List<TipoEncuestaRespuestaDTO>>> Listar(IRepositorioTipoEncuestaRespuestas repositorio
            , IMapper mapper)
        {
            var tipoEncuestaRespuestas = await repositorio.Listar();
            var tipoEncuestaRespuestasDTO = mapper.Map<List<TipoEncuestaRespuestaDTO>>(tipoEncuestaRespuestas);
            return TypedResults.Ok(tipoEncuestaRespuestasDTO);
        }

        static async Task<Results<Ok<TipoEncuestaRespuestaDTO>, NotFound>> Obtener(IRepositorioTipoEncuestaRespuestas repositorio
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

            var tipoEncuestaRespuestas = new TipoEncuestaRespuestaDTO()
            {
                ID_TIPO_ENCUESTA_PREGUNTA = result.First().ID_TIPO_ENCUESTA_PREGUNTA,
                NOMBRE = result.First().NOMBRE,
            };

            var tipoEncuestaRespuestasDTO = mapper.Map<TipoEncuestaRespuestaDTO>(tipoEncuestaRespuestas);
            return TypedResults.Ok(tipoEncuestaRespuestasDTO);
        }


    }
}
