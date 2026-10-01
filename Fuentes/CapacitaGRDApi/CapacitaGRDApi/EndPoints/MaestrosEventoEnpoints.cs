using AutoMapper;
using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class MaestrosEventoEnpoints
    {
        public static RouteGroupBuilder MapMaestrosEventos(this RouteGroupBuilder group)
        {
            group.MapGet("/", Listar);
     
            return group;
        }
        static async Task<Ok<MaestroEventoDTO>> Listar(
            IRepositorioTipoEventos repositorioTipoEventos
            , IRepositorioModalidad repositorioModalidad
            , IRepositorioEstados repositorioEstados

            , IRepositorioEncuestas repositorioEncuestas
            , IRepositorioCuestionarios repositorioCuestionarios
            , IRepositorioDistritos repositorioDistritos
            , IRepositorioDocumentos repositorioDocumentos

            , IMapper mapper)
        {
            var tiposEventos = await repositorioTipoEventos.Listar();
            var tiposEventosDTO = mapper.Map<List<TipoEventoDTO>>(tiposEventos);

            var modalidades = await repositorioModalidad.Listar();
            var modalidadesDTO = mapper.Map<List<ModalidadDTO>>(modalidades);

            var estados = await repositorioEstados.Listar();
            var estadosDTO = mapper.Map<List<EstadoDTO>>(estados);

            var encuestas = await repositorioEncuestas.Listar();
            var encuestasDTO = mapper.Map<List<EncuestaDTO>>(encuestas);

            var cuestionarios = await repositorioCuestionarios.Listar();
            var cuestionariosDTO = mapper.Map<List<CuestionarioDTO>>(cuestionarios);

            var distritos = await repositorioDistritos.Listar();
            var distritosDTO = mapper.Map<List<DistritoDTO>>(distritos);

            var documentos = await repositorioDocumentos.Listar();
            var documentoDTO = mapper.Map<List<DocumentoDTO>>(documentos);

            var MaestroEventoDTO = new MaestroEventoDTO{
                EstadoEvento = estadosDTO,
                ModalidadEvento = modalidadesDTO,
                TipoEvento = tiposEventosDTO,
                Encuestas = encuestasDTO,
                Cuestionarios= cuestionariosDTO,
                Distritos= distritosDTO,
                Documentos = documentoDTO,
            };

            return TypedResults.Ok(MaestroEventoDTO);
        }
         

    }
}
