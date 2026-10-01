using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioTipoDocumentos
    {
        Task<List<TipoDocumento>> Listar();

        Task<IEnumerable<TipoDocumento>> Obtener(int id);
 

    }
}

