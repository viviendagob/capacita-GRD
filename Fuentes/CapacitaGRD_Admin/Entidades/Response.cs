
namespace CapacitaGRD_Admin.Entidades
{
    public class Response<T>
    {
        public bool? estado { set; get; } = true;
        public string titulo { set; get; }
        public string mensaje { set; get; }
        public T data { get; set; }

    }
}
