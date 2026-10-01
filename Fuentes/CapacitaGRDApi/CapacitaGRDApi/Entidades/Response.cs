
namespace CapacitaGRDApi.Entidades
{
    public class Response<T>
    {
        public bool? success { set; get; } = true;
        public string titulo { set; get; }
        public string mensaje { set; get; }

        public T data { get; set; } 

    }
}
