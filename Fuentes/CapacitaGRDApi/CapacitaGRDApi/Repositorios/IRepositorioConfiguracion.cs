using CapacitaGRDApi.Entidades;
 

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioConfiguracion
    {
        Task<IEnumerable<Configuracion>> Obtener(int id);
         Task<int> Actualizar(Configuracion configuracion);
 
    }
}

