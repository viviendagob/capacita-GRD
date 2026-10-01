using CapacitaGRDApi.DTOs;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioReportes
    {
        Task<List<ReporteParticipanteDTO>> ReporteParticipantes(int idEvento);
    }
}
