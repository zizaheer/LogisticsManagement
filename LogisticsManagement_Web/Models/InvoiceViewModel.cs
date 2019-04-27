using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class InvoiceViewModel
    {
    }


    public class PendingWaybillsForInvoice
    {
        public string  WaybillNumber { get; set; }
        public int  BillerCustomerId { get; set; }
        public string  BillerName { get; set; }
        public string  BillerEmail { get; set; }
        public string  BillerDepartment { get; set; }
        public string  DeliveryStatus { get; set; }
        public decimal SingleOrderCost { get; set; }
        public decimal ReturnOrderCost { get; set; }
        public decimal  TotalWayBillOrderCost { get; set; }
    }
}
