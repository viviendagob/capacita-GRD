using CapacitaGRDApi.Entidades;
using static CapacitaGRDApi.Util.Paginado;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioPersonas
    {
        Task<List<Persona>> Listar();

        Task<List<Persona>> Paginar(Filter filter);

        Task<List<Persona>> ListarFiltrado(string where, string order);

        Task<IEnumerable<Persona>> Obtener(int id);
         
        Task<IEnumerable<Persona>> Existe(int tipo, string documento);

        Task<int> Agregar(Persona persona);

        Task<int> Actualizar(Persona persona);

        Task<int> Eliminar(int id);

        Task<int> Reiniciar(int id);

    }
}

