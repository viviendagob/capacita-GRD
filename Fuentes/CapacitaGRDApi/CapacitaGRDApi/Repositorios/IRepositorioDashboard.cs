using CapacitaGRDApi.DTOs;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioDashboard
    {
        Task<DashboardDTO> Kpis(DashboardFiltroDTO filtro);
    }
}
