using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioEncuestaRespuestas
    {
        Task<List<EncuestaRespuesta>> Listar(int id);

        Task<IEnumerable<EncuestaRespuesta>> Obtener(int idEncuesta, int id);

        Task<int> Agregar(EncuestaRespuesta encuestaRespuesta);

        Task<int> Actualizar(EncuestaRespuesta encuestaRespuesta);

        Task<int> Eliminar(int id);

    }
}

