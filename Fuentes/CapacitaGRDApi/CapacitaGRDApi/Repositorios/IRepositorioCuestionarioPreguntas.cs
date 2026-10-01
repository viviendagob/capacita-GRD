using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioCuestionarioPreguntas
    {
        Task<List<CuestionarioPregunta>> Listar(int idCuestionario);

        Task<int> Agregar(CuestionarioPregunta pregunta);

        Task<int> EliminarPorCuestionario(int idCuestionario);

    }
}
