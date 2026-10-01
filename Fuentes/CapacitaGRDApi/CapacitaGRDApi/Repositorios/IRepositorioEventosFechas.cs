using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEventosFechas
    {
        Task<List<EventoFecha>> Listar(int id);

        Task<IEnumerable<EventoFecha>> Obtener(int id, DateTime fecha);

        Task<int> Agregar(EventoFecha eventoFecha);

        Task<int> Actualizar(EventoFecha eventoFecha);

        Task<int> Eliminar(int id, DateTime fecha);

    }
}

