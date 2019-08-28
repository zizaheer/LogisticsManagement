using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_PaidInvoice
    {
        public int InvoiceId { get; set; }
        public int BillToCustomerId { get; set; }
        public string BillerName { get; set; }
        public string InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }
        public int? BankId { get; set; }
        public string BankName { get; set; }
        public string MorePaymentInfo { get; set; }
    }
}
