using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_OrderReadyForInvoice
    {
        public int OrderId { get; set; }
        public string WayBillNumber { get; set; }
        public int OrderTypeId { get; set; }
        //public string OrderType { get; set; }
        public int BillToCustomerId { get; set; }
        public string BillerName { get; set; }
        //public string  BillerEmail { get; set; }
        public string BillerDepartment { get; set; }

        public decimal TotalOrderCost { get; set; }
        public decimal ReturnOrderCost { get; set; }
        public string DeliveryStatus { get; set; }
        public int BillingAddressId { get; set; }
        public int MailingAddressId { get; set; }
    }
}
