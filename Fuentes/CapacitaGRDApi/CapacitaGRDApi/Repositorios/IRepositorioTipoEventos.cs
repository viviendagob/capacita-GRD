using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioTipoEventos
    {
        Task<List<TipoEvento>> Listar();

        Task<IEnumerable<TipoEvento>> Obtener(int id);
 

    }
}

