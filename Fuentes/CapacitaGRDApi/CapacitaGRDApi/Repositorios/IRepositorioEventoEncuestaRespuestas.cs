using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEventoEncuestaRespuestas
    {
        Task AgregarLote(List<EventoEncuestaRespuesta> respuestas);
        Task<bool> Completada(int idEvento, int idPersona);
        Task<EstadisticaEncuestaDTO> Estadisticas(int idEvento);
    }
}
