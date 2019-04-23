using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class DispatchedOrderViewModel
    {
        public int OrderId { get; set; }
        public string WayBillNumber { get; set; }
        public int BillerId { get; set; }
        public string BillerName { get; set; }
        public string BillerAddress { get; set; }
        public int ShipperId { get; set; }
        public string ShipperName { get; set; }
        public string ShipperAddress { get; set; }
        public int ConsigneeId { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePhone { get; set; }
        public string TruckNo { get; set; }
        public string LicensePlateNo { get; set; }
        public bool? IsOrderDispatched { get; set; }
        public bool? IsOrderPickedup { get; set; }
        public bool? IsOrderPassedOn { get; set; }
        public bool? IsOrderDelivered { get; set; }

        public DateTime? DispatchDatetime { get; set; }
        public DateTime? PickupDatetime { get; set; }
        public DateTime? PassOffDatetime { get; set; }
        public DateTime? DeliverDatetime { get; set; }

    }
}
