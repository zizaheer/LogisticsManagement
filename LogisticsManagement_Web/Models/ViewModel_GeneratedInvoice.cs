using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_GeneratedInvoice
    {
        public int InvoiceId { get; set; }
        public string InvoiceDateString { get; set; }
        public int BillToCustomerId { get; set; }
        public string BillerName { get; set; }
        public string BillerDepartment { get; set; }
        public string WayBillNumbers { get; set; }
        public decimal TotalInvoiceAmnt { get; set; }
        public DateTime PaymentDueDate { get; set; }
    }
}

