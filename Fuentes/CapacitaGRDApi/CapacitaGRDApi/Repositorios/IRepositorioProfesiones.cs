using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioProfesiones
    {
        Task<List<Profesion>> Listar();

        Task<List<Profesion>> Paginar(Filter filter);

        Task<IEnumerable<Profesion>> Obtener(int id);

        Task<int> Agregar(Profesion profesion);

        Task<int> Actualizar(Profesion profesion);

        Task<int> Eliminar(int id);

    }
}

