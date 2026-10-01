using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioCargos
    {
        Task<List<Cargo>> Listar();

        Task<List<Cargo>> Paginar(Filter filter);

        Task<IEnumerable<Cargo>> Obtener(int id);

        Task<int> Agregar(Cargo cargo);

        Task<int> Actualizar(Cargo cargo);

        Task<int> Eliminar(int idCargo);

    }
}

