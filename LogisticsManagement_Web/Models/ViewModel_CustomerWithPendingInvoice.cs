using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_CustomerWithPendingInvoice
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAdress { get; set; }
        public string CustomerPhone { get; set; }
        public decimal? TotalDue { get; set; }
        public int NumberOfInvoices { get; set; }
    }
}
