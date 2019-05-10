using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class InvoiceViewModel
    {
        public int InvoiceNumber { get; set; }
        public int BillToCustomerId { get; set; }
        public string BillerName { get; set; }
        public string BillerDepartment { get; set; }
        public string WayBillNumbers { get; set; }
        public decimal TotalInvoiceAmnt { get; set; }
        public DateTime PaymentDueDate { get; set; }
    }


    public class PendingWaybillsForInvoice
    {
        public int OrderId { get; set; }
        public string WayBillNumber { get; set; }
        public int OrderTypeId { get; set; }
        //public string OrderType { get; set; }
        public int BillToCustomerId { get; set; }
        public string  BillerName { get; set; }
        //public string  BillerEmail { get; set; }
        public string  BillerDepartment { get; set; }
       
        public decimal TotalOrderCost { get; set; }
        public decimal ReturnOrderCost { get; set; }
        public string DeliveryStatus { get; set; }
        public int BillingAddressId { get; set; }
        public int MailingAddressId { get; set; }
    }
}




//Error converting value "[{"OrderId":29,"WayBillNumber":"1022","OrderType":"Single","BillToCustomerId":1649,"BillerName":"2 SOURCE MKTNG","TotalOrderCost":231.23,"BillingAddressId":1609,"MailingAddressId":1609},{"OrderId":30,"WayBillNumber":"1022","OrderType":"Return","BillToCustomerId":1649,"BillerName":"2 SOURCE MKTNG","TotalOrderCost":110.25,"BillingAddressId":1609,"MailingAddressId":1609}]" to type 'System.Collections.Generic.List`1[LogisticsManagement_Web.Models.PendingWaybillsForInvoice]'. Path '', line 1, position 417.


//SET @ReturnedResult = (SELECT O.OrderId, O.WayBillNumber, (CASE WHEN O.OrderTypeId = 1 THEN 'Single' WHEN O.OrderTypeId = 2 THEN 'Return' END) AS OrderType, O.BillToCustomerId
//, C.CustomerName AS BillerName, O.DepartmentName AS BillerDepartment, (O.TotalOrderCost + O.TotalAdditionalServiceCost) AS TotalOrderCost
//, C.BillingAddressId, C.MailingAddressId FROM Lms_Order O 