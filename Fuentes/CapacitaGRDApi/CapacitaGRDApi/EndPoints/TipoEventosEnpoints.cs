using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class TipoEventosEnpoints
    {
        public static RouteGroupBuilder MapTipoEventos(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
           
            return group;
        }

        static async Task<Ok<List<TipoEventoDTO>>> Listar(
                IRepositorioTipoEventos repositorio
            , IMapper mapper)
        {
            var tipoeventos = await repositorio.Listar();
            var tipoEventoDTO = mapper.Map<List<TipoEventoDTO>>(tipoeventos);
            return TypedResults.Ok(tipoEventoDTO);
        }

        static async Task<Results<Ok<TipoEventoDTO>, NotFound>> Obtener(
                IRepositorioTipoEventos repositorio
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

            var tipoevento = new TipoEvento()
            {
                ID_TIPO_EVENTO = result.First().ID_TIPO_EVENTO,
                NOMBRE = result.First().NOMBRE,
            };

            var tipoeventoDTO = mapper.Map<TipoEventoDTO>(tipoevento);
            return TypedResults.Ok(tipoeventoDTO);
        }


    }
}
