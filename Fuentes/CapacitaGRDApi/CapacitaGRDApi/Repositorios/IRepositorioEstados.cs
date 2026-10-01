using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEstados
    {
        Task<List<Estado>> Listar();

        Task<IEnumerable<Estado>> Obtener(int id);
 

    }
}

