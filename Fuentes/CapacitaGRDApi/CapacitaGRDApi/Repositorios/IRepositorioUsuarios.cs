using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioUsuarios
    {
        Task<List<Usuarios>> Listar();

        Task<List<Usuarios>> Paginar(Filter filter);

        Task<IEnumerable<Usuarios>> Obtener(int id);

        Task<int> Agregar(Usuarios pais);

        Task<int> Actualizar(Usuarios pais);

        Task<int> Eliminar(int id);

        Task<IEnumerable<Usuario>> Busqueda(string usuario, string clave);
    }
}

