using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class TipoDocumentosEndPoints
    {
        public static RouteGroupBuilder MapTipoDocumentos(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar).AllowAnonymous();
            group.MapGet("/{id:int}", Obtener);
           
            return group;
        }

        static async Task<Ok<List<TipoDocumentDTO>>> Listar(IRepositorioTipoDocumentos repositorio
            , IMapper mapper)
        {
            var tipoDocumentos = await repositorio.Listar();
            var tipoDocumentosDTO = mapper.Map<List<TipoDocumentDTO>>(tipoDocumentos);
            return TypedResults.Ok(tipoDocumentosDTO);
        }

        static async Task<Results<Ok<TipoDocumentDTO>, NotFound>> Obtener(IRepositorioTipoDocumentos repositorio
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

            var tipodocumento = new TipoDocumento()
            {
                ID_TIPO_DOCUMENTO = result.First().ID_TIPO_DOCUMENTO,
                NOMBRE = result.First().NOMBRE,
            };

            var tipoDocumentosDTO = mapper.Map<TipoDocumentDTO>(tipodocumento);
            return TypedResults.Ok(tipoDocumentosDTO);
        }


    }
}

