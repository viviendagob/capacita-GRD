using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioDocumentos
    {
        Task<List<Documento>> Listar();

        Task<List<Documento>> Paginar(Filter filter);

        Task<IEnumerable<Documento>> Obtener(int id);

        Task<int> Agregar(Documento cargo);

        Task<int> Actualizar(Documento cargo);

        Task<int> Eliminar(int id);

    }
}

