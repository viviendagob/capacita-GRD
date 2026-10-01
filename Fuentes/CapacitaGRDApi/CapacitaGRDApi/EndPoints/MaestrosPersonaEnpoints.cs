using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class MaestrosPersonaEnpoints
    {
        public static RouteGroupBuilder MapMaestrosPersona(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar).AllowAnonymous();
     
            return group;
        }
        static async Task<Ok<MaestroPersonaDTO>> Listar(
            IRepositorioTipoDocumentos repositorioTipoDocumentos
            , IRepositorioPais repositorioPais
            , IRepositorioProfesiones repositorioProfesiones
            , IRepositorioCargos repositorioCargos
            , IRepositorioDistritos repositorioDistritos
            , IRepositorioEntidades repositorioEntidades
            , IMapper mapper)
        {
            var tipoDocumentos = await repositorioTipoDocumentos.Listar();
            var tipoDocumentosDTO = mapper.Map<List<TipoDocumentDTO>>(tipoDocumentos);

            var pais = await repositorioPais.Listar();
            var paisDTO = mapper.Map<List<PaisDTO>>(pais);

            var profesiones = await repositorioProfesiones.Listar();
            var profesionesDTO = mapper.Map<List<ProfesionDTO>>(profesiones);

            var distritos = await repositorioDistritos.Listar();
            var distritosDTO = mapper.Map<List<DistritoDTO>>(distritos);

            var cargos = await repositorioCargos.Listar();
            var cargosDTO = mapper.Map<List<CargoDTO>>(cargos);

            var entidades  = await repositorioEntidades.Listar();
            var entidadesDTO = mapper.Map<List<EntidadDTO>>(entidades);

            var maestroPersonaDTO = new MaestroPersonaDTO
            {
                TipoDocumento = tipoDocumentosDTO,
                Pais = paisDTO,
                Profesion = profesionesDTO,
                Distrito = distritosDTO, 
                Cargo = cargosDTO,
                Entidad = entidadesDTO,
            };

            return TypedResults.Ok(maestroPersonaDTO);
        }
         

    }
}

