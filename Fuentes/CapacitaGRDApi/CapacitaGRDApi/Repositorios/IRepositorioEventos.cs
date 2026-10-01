using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEventos
    {
        Task<List<Evento>> Listar();

        Task<List<Evento>> Paginar(Filter filter);

        Task<IEnumerable<Evento>> Obtener(int id);

        Task<int> Agregar(Evento evento);

        Task<int> Actualizar(Evento evento);

        Task<int> Eliminar(int id);

        Task<EventoEstadisticaDTO> Estadisticas();

    }
}

