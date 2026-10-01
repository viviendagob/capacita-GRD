using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioPersonasData
    {
       
        Task<IEnumerable<PersonaData>> Obtener(int id);

        Task<IEnumerable<PersonaData>> ObtenerPorPersona(int idPersona);

        
        Task<int> Agregar(PersonaData personaData);

        Task<int> Eliminar(int id);

        Task<List<string>> ListarAreasLaborales();

    }
}

