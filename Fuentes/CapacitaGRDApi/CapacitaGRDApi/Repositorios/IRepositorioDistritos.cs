using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioDistritos
    {
        Task<List<Distrito>> Listar();

        Task<List<Distrito>> Paginar(Filter filter);

        Task<IEnumerable<Distrito>> Obtener(int id);

        Task<int> Agregar(Distrito distrito);

        Task<int> Actualizar(Distrito distrito);

        Task<int> Eliminar(int id);

    }
}

