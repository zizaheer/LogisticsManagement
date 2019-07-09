using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_OrderDispatched
    {
        public int OrderId { get; set; }
        public int OrderTypeId { get; set; }
        public string OrderTypeFlag { get; set; }

        public string WayBillNumber { get; set; }

        public string OrderDateString { get; set; }

        public int DeliveryOptionId { get; set; }
        public string DeliveryOptionName { get; set; }
        public string DeliveryOptionCode { get; set; }

        public int BillerCustomerId { get; set; }
        public string BillerCustomerName { get; set; }
        public string BillerAddress { get; set; }

        public int ShipperCustomerId { get; set; }
        public string ShipperCustomerName { get; set; }
        public string ShipperAddress { get; set; }
        public int ConsigneeCustomerId { get; set; }
        public string ConsigneeCustomerName { get; set; }
        public string ConsigneeAddress { get; set; }

        public int? ServiceProviderEmployeeId { get; set; }
        public string ServiceProviderEmployeeName { get; set; }
        public string TruckNo { get; set; }
        public string LicensePlateNo { get; set; }

        public string OrderStatus { get; set; }

        public string CustomerRefNumber { get; set; }
        public int UnitTypeId { get; set; }
        public string UnitTypeName { get; set; }
        public int? UnitQuantity { get; set; }
        public int? SkidQuantity { get; set; }
        public int? TotalPiece { get; set; }
        public int? WeightScaleId { get; set; }
        public decimal? WeightTotal { get; set; }
        public string SpcIns { get; set; }
        public bool IsInvoiced { get; set; }

        public bool? IsOrderDispatched { get; set; }
        public DateTime? DispatchDatetime { get; set; }
        public int? DispatchedToEmployeeId { get; set; }
        public string DispatchedToEmployeeName { get; set; }
        public string DispatchedToEmployeePhone { get; set; }
        public string DispatchedToEmployeeEmail { get; set; }

        public bool? IsOrderPickedup { get; set; }
        public DateTime? PickupDatetime { get; set; }
        public decimal? PickupWaitTime { get; set; }

        public bool? IsOrderPassedOn { get; set; }
        public DateTime? PassOnDatetime { get; set; }
        public decimal? PassOnWaitTime { get; set; }
        public int? PassedOnToEmployeeId { get; set; }
        public string PassOnToEmployeeName { get; set; }
        public string PassOnToEmployeePhone { get; set; }
                          
        public bool? IsOrderDelivered { get; set; }
        public DateTime? DeliverDatetime { get; set; }
        public decimal? DeliveryWaitTimeInHour { get; set; }
        public string ProofOfDeliveryNote { get; set; }
        public string ReceivedByName { get; set; }
        public string ReceivedBySignature { get; set; }

        public string RowColorCode { get; set; }
    }
}
