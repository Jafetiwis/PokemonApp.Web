using System.Net;
using System.Net.Mail;

namespace PokemonApp.Web.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string mensajeHtml)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");

            var mail = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"]!, smtpSettings["SenderName"]),
                Subject = asunto,
                Body = mensajeHtml,
                IsBodyHtml = true
            };

            mail.To.Add(destinatario);

            using var smtpClient = new SmtpClient(smtpSettings["Server"])
            {
                Port = int.Parse(smtpSettings["Port"]!),
                Credentials = new NetworkCredential(smtpSettings["SenderEmail"], smtpSettings["Password"]),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(mail);
        }
    }
}
