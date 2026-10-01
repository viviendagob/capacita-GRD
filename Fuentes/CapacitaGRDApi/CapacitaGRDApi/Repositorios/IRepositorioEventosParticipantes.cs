using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEventosParticipantes
    {
        Task<IEnumerable<EventoParticipante>> Existe(int idEvento, int idPersona);

        Task<List<EventoParticipante>> Eventos(int idPersona);

        Task<int> Agregar(EventoParticipante eventoParticipante);

        Task<List<ReporteInscritoDTO>> ReporteInscritos(int idEvento);


    }
}

