using CapacitaGRDApi.Entidades;

namespace CapacitaGRDApi.Repositorios
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, string adjunto = "");
         
    }
}

