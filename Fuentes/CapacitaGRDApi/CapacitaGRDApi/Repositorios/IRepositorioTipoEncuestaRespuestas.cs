using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioTipoEncuestaRespuestas
    {
        Task<List<TipoEncuestaRespuesta>> Listar();

        Task<IEnumerable<TipoEncuestaRespuesta>> Obtener(int id);
 

    }
}

