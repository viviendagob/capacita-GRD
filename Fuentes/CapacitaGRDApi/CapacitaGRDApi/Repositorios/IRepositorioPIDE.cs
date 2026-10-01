using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IRepositorioPIDE
    {
         
        Task<Persona> ValidaDNI(string dni);

        Task<Persona> ValidaCE(string ce);


    }
}

