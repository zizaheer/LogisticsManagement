using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_Report_CustomerDue
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string ProvinceCode { get; set; }
        public string PostCode { get; set; }
        public decimal? TotalAmountPayable { get; set; }
        public string InvoiceDueDays { get; set; }

        public List<ViewModel_InvoiceDueData> InvoiceData { get; set; }
    }

    public class ViewModel_InvoiceDueData
    {
        public int InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public decimal TotalInvoiceAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? BalanceDueAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDate { get; set; }
        public string ChqNo { get; set; }
        public string ChqAmount { get; set; }
        public string ChqDate { get; set; }
        public string BankShortName { get; set; }
    }
}
