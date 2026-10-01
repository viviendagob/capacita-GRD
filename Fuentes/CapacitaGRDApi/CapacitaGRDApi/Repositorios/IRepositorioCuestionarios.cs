using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioCuestionarios
    {
        Task<List<Cuestionario>> Listar();

        Task<List<Cuestionario>> Paginar(Filter filter);

        Task<IEnumerable<Cuestionario>> Obtener(int id);

        Task<int> Agregar(Cuestionario cuestionario);

        Task<int> Actualizar(Cuestionario cuestionario);

        Task<int> Eliminar(int id);

    }
}

