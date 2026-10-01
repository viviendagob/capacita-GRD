using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioPais
    {
        Task<List<Pais>> Listar();

        Task<List<Pais>> Paginar(Filter filter);

        Task<IEnumerable<Pais>> Obtener(int id);

        Task<int> Agregar(Pais pais);

        Task<int> Actualizar(Pais pais);

        Task<int> Eliminar(int id);

    }
}

