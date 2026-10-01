using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEntidades
    {
        Task<List<Entidad>> Listar();

        Task<IEnumerable<Entidad>> Obtener(int id);
 

    }
}

