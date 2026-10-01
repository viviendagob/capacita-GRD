using CapacitaGRDApi.DTOs;
using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEventosAsistencias
    {
        Task<IEnumerable<EventoAsistencia>> Existe(EventoAsistencia eventoAsistencia);

        Task<int> Agregar(EventoAsistencia eventoAsistencia);

        Task<List<EventoAsistencia>> Listar(int idEvento, int idPersona);

        Task<List<ReporteAsistenciaDTO>> ListarPorEvento(int idEvento);


    }
}

