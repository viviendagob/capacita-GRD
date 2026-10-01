using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioSecciones
    {
        Task<List<Seccion>> Listar();

        Task<IEnumerable<Seccion>> Obtener(int id);

        Task<int> Agregar(Seccion cargo);

        Task<int> Actualizar(Seccion cargo);

        Task<int> Eliminar(int id);

    }
}

