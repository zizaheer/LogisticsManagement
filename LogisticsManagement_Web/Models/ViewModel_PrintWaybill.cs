using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_PrintWaybill
    {
        public string WaybillNumber { get; set; }
        public string WayBillDate { get; set; }
        public int BillerCustomerId { get; set; }
        public string CustomerRefNo { get; set; }
        public string CargoCtlNo { get; set; }
        public string AwbContainerNo { get; set; }
        public string PickupRefNo { get; set; }
        public string BillerCustomerName { get; set; }
        public string OrderedByName { get; set; }
        public string DeliveryOptionShortCode { get; set; }

        public string OrderBasePrice { get; set; }
        public string OrderDiscountAmount { get; set; }
        public string FuelSurcharge { get; set; }
        public string OrderTaxAmount { get; set; }
        //public string TotalOrderCost { get; set; }
        //public string TotalOrderCostWithoutTaxAndFuel { get; set; }
        public string AdditionalServiceCost { get; set; }
        public string GrandTotalOrderCost { get; set; }

        public string ShipperCustomerName { get; set; }
        public string ShipperCustomerAddressLine1 { get; set; }
        public string ShipperCustomerAddressLine2 { get; set; }
        public string ConsigneeCustomerName { get; set; }
        public string ConsigneeCustomerAddressLine1 { get; set; }
        public string ConsigneeCustomerAddressLine2 { get; set; }
        public int TotalSkidPieces { get; set; }
        public string UnitTypeName { get; set; }
        public string UnitTypeShortCode { get; set; }
        public int? UnitQuantity { get; set; }
        public int? SkidQuantity { get; set; }
        public int? TotalPiece { get; set; }
        public string WeightScaleShortCode { get; set; }
        public string WeightTotal { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string PUDriverName { get; set; }
        public string DeliveryDriverName { get; set; }
        public string WaybillComments { get; set; }
    }
}
