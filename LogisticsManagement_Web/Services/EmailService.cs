using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Services
{
    public class EmailService : IEmailService
    {
        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public EmailService(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }


        public async Task SendEmail(string email, string subject, string message)
        {
            using (var client = new SmtpClient())
            {

                var credential = new NetworkCredential
                {
                    UserName = _configuration["Email:Email"],
                    Password = _configuration["Email:Password"]
                };

                client.Credentials = credential;
                client.Host = _configuration["Email:Host"];
                client.Port = int.Parse(_configuration["Email:Port"]);
                client.EnableSsl = true;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(email);
                    emailMessage.From = new MailAddress(_configuration["Email:Email"]);
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    
                    client.Send(emailMessage);
                }

            }

            await Task.CompletedTask;

        }
    }
}
