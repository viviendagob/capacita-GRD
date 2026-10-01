using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEventoConstancias
    {
        Task<List<ParticipanteConstanciaDTO>> ListarElegibles(int idEvento);
        Task<EventoConstancia?> Obtener(int idConstancia);
        Task<EventoConstancia?> ObtenerPorEventoPersona(int idEvento, int idPersona);
        Task<EventoConstancia?> ObtenerPorCodigo(string codigo);
        Task<List<ConstanciaDTO>> Buscar(string? documento, int? idEvento, string? codigo);
        Task<int> Agregar(EventoConstancia constancia);
        Task RegistrarDescarga(int idConstancia);
        Task Anular(int idConstancia, string motivo, string usuario);
    }
}
