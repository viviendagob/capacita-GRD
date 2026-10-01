using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioCuestionarioPreguntaRespuestas
    {
        Task<List<CuestionarioPreguntaRespuesta>> Listar(int idCuestionarioPregunta);

        Task<int> Agregar(CuestionarioPreguntaRespuesta respuesta);

    }
}
