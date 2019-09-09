using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
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


        public async Task SendEmail(string sentToFirstName, string sendToEmailAddress, int emailType, string attachmentFilePath = "", string emailSubject = "", string emailMessageBody = "")
        {
            try
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

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.To.Add(sendToEmailAddress);
                        mailMessage.From = new MailAddress(_configuration["Email:Email"]);

                        if (emailType == 1)
                        {
                            mailMessage.Subject = _configuration["WaybillEmail:Subject"];
                            mailMessage.Body = _configuration["WaybillEmail:Salutation"];
                            mailMessage.Body = mailMessage.Body + sentToFirstName + Environment.NewLine;
                            mailMessage.Body = mailMessage.Body + emailMessageBody + Environment.NewLine;
                            mailMessage.Body = mailMessage.Body + _configuration["WaybillEmail:BodyText"];
                        }
                        else if (emailType == 2)
                        {
                            mailMessage.Subject = _configuration["InvoiceEmail:Subject"];
                            mailMessage.Body = _configuration["InvoiceEmail:Salutation"];
                            mailMessage.Body = mailMessage.Body + sentToFirstName + Environment.NewLine;
                            mailMessage.Body = mailMessage.Body + emailMessageBody + Environment.NewLine;
                            mailMessage.Body = mailMessage.Body + _configuration["InvoiceEmail:BodyText"];
                        }

                        if (!string.IsNullOrEmpty(attachmentFilePath))
                        {
                            Attachment attachment = new Attachment(attachmentFilePath);
                            mailMessage.Attachments.Add(attachment);
                        }

                        client.Send(mailMessage);
                    }

                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
