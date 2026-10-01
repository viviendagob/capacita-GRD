using CapacitaGRDApi.Util;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CapacitaGRDApi.Repositorios
{
    public class EmailSender : IEmailSender
    {

        private SmtpClient Cliente { get; }
        private EmailSenderOptions Options { get; }
         
        public EmailSender(IOptions<EmailSenderOptions> options)
        {
            Options = options.Value;
            Cliente = new SmtpClient()
            {
                Host = Options.Host,
                Port = Options.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Options.Email, Options.Password),
                EnableSsl = Options.EnableSsl,
            };
        }

        public Task SendEmailAsync(string email, string subject, string message, string adjunto = "")
        {
            var correo = new MailMessage(from: Options.Email, to: email, subject: subject, body: message);

            if (adjunto != "")
                correo.Attachments.Add(new Attachment(adjunto));

            correo.IsBodyHtml = true;
            return Cliente.SendMailAsync(correo);
        }

    }
}
