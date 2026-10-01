using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class ModalidadesEnpoints
    {
        public static RouteGroupBuilder MapModalidades(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
            group.MapGet("/{id:int}", Obtener);
           
            return group;
        }

        static async Task<Ok<List<ModalidadDTO>>> Listar(
                IRepositorioModalidad repositorio
            , IMapper mapper)
        {
            var modalidades = await repositorio.Listar();
            var modalidadesDTO = mapper.Map<List<ModalidadDTO>>(modalidades);
            return TypedResults.Ok(modalidadesDTO);
        }

        static async Task<Results<Ok<ModalidadDTO>, NotFound>> Obtener(
                IRepositorioModalidad repositorio
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

            var modalidad = new Modalidad()
            {
                ID_MODALIDAD = result.First().ID_MODALIDAD,
                NOMBRE = result.First().NOMBRE,
            };

            var modalidadDTO = mapper.Map<ModalidadDTO>(modalidad);
            return TypedResults.Ok(modalidadDTO);
        }


    }
}
