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
       
        public string TruckNo { get; set; }
        public string LicensePlateNo { get; set; }

        public bool? IsOrderDispatched { get; set; }
        public DateTime? DispatchDatetime { get; set; }
        public int? DispatchedEmployeeId { get; set; }
        public string DispatchedEmployeeName { get; set; }
        public string DispatchedEmployeePhone { get; set; }

        public bool? IsOrderPickedup { get; set; }
        public DateTime? PickupDatetime { get; set; }
        public decimal? PickupWaitTime { get; set; }

        public bool? IsOrderPassedOn { get; set; }
        public DateTime? PassOnDatetime { get; set; }
        public decimal? PassOnWaitTime { get; set; }
        public int? PassOnEmployeeId { get; set; }
        public string PassOnEmployeeName { get; set; }
        public string PassOnEmployeePhone { get; set; }
                          
        public bool? IsOrderDelivered { get; set; }
        public DateTime? DeliverDatetime { get; set; }
        public decimal? DeliveryWaitTimeInHour { get; set; }
        public string ProofOfDeliveryNote { get; set; }
        public string ReceivedByName { get; set; }
        //public byte[] ReceivedBySignature { get; set; }
        public string ReceivedBySignature { get; set; }

    }
}
