using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CapacitaGRDApi.EndPoints
{
    public static class DashboardEndpoints
    {
        public static RouteGroupBuilder MapDashboard(this RouteGroupBuilder group)
        {
            group.MapPost("/kpis", Kpis);

            return group;
        }

        static async Task<Ok<DashboardDTO>> Kpis(
            DashboardFiltroDTO filtro
            , IRepositorioDashboard repositorio)
        {
            var resultado = await repositorio.Kpis(filtro);
            return TypedResults.Ok(resultado);
        }
    }
}
