using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_SalesReport
    {
        public int BillToCustomerId { get; set; }
        public string BillToCustomerName { get; set; }
        public string ReportDuration { get; set; }
        public decimal? TotalWithoutHst { get; set; }
        public decimal? TotalHst { get; set; }
        public decimal? TotalWithHst { get; set; }
        public decimal? TotalAmountRecieved { get; set; }
        public decimal? TotalAmountDue { get; set; }
    }
}
