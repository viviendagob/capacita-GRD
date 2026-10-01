using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEncuestas
    {
        Task<List<Encuesta>> Listar();

        Task<IEnumerable<Encuesta>> Obtener(int id);

        Task<List<Encuesta>> Paginar(Filter filter);

        Task<int> Agregar(Encuesta encuesta);

        Task<int> Actualizar(Encuesta encuesta);

        Task<int> Eliminar(int id);

        Task<int> EliminarRespuestas(int id);

    }
}

