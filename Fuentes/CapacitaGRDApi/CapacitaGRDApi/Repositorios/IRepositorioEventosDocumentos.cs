using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEventosDocumentos
    {
        Task<List<EventoDocumento>> Listar(int id);

        Task<int> Agregar(EventoDocumento documento);
         
        Task<int> Eliminar(int id);

    }
}

