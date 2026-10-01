namespace CapacitaGRDApi.DTOs
{
    public class UsuarioDTO
    {
        public int ID_USUARIO { get; set; }

        public String USUARIO { get; set; } = null!;

        public String CLAVE { get; set; } = null!;

        public int ESTADO { get; set; }
    }
}
