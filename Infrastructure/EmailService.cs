using FluentEmail.Core;
using System.Threading.Tasks;

public class EmailService
{
    //private readonly IFluentEmail _fluentEmail;

    //public EmailService(IFluentEmail fluentEmail)
    //{
    //    _fluentEmail = fluentEmail;
    //}

    //public async Task SendEmailAsync(string recipientEmail, string subject, string messageBody)
    //{
       
    //        var response = await _fluentEmail
    //            .To(recipientEmail)
    //            .Subject(subject)
    //            .Body(messageBody, isHtml: true)
    //            .SendAsync();
    //}

}





//using FluentEmail.Core;
//using System.Net;
//using System.Net.Mail;
//using System.Threading.Tasks;
//namespace FitnessTracker.Infrastructure
//{
//    public interface IEmailService
//    {
//        public Task<bool> SendEmailAsync(string recipientEmail, string subject, string messageBody);
//    }
//    public class EmailService : IEmailService
//    {
//        private readonly IConfiguration _configuration;

//        public EmailService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string messageBody)
//        {
//            try
//            {
//                string host = _configuration["SmtpSettings:Host"];
//                int.TryParse(_configuration["SmtpSettings:Port"], out int port);
//                string username = _configuration["SmtpSettings:Username"];
//                string password = _configuration["SmtpSettings:Password"];

//                var smtpClient = new SmtpClient(host)
//                {
//                    Port = port,
//                    Credentials = new NetworkCredential(username, password),
//                    EnableSsl = true
//                };

//                var mailMessage = new MailMessage
//                {
//                    From = new MailAddress(username,"FitnessTracker_Mail"),
//                    Subject = subject,
//                    Body = messageBody,
//                    IsBodyHtml = true,
//                };
//                mailMessage.To.Add(recipientEmail);

//                await smtpClient.SendMailAsync(mailMessage);

//                return true;
//            }
//            catch (Exception)
//            {                
//                throw;
//            }

//        }
//    }
//}
