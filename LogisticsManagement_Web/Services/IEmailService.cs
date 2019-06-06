using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Services
{
    public interface IEmailService
    {
        Task SendEmail(string sentToFirstName, string sendToEmailAddress, int emailType, string attachmentFilePath = "", string emailSubject = "", string emailMessage = ""); // 1 - Invoice, 2 - Waybill
    }
}
