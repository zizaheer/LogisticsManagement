using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_PrintInvoice
    {
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int BillerCustomerId { get; set; }
        public string BillerCustomerAddress { get; set; }
        public string BillerPostCode { get; set; }
        public int Term { get; set; }

        List<ViewModel_PrintWaybill> viewModelWaybills { get; set; }
    }
}
