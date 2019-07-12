using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_PrintInvoice
    {
        public List<ViewModel_InvoiceBiller> viewModelInvoiceBillers { get; set; }

        public List<ViewModel_PrintWaybill> viewModelWaybills { get; set; }
    }


    public class ViewModel_InvoiceBiller {

        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int BillerCustomerId { get; set; }
        public string BillerCustomerName { get; set; }
        public string BillerCustomerAddressLine { get; set; }
        public string BillerCustomerCityLine { get; set; }
        public string BillerPostCode { get; set; }
        public string BillerDepartment { get; set; }
        public byte? Term { get; set; }
    }
}


