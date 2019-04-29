using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Services
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message);
    }
}
