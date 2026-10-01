using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioModalidad
    {
        Task<List<Modalidad>> Listar();

        Task<IEnumerable<Modalidad>> Obtener(int id);
 

    }
}

