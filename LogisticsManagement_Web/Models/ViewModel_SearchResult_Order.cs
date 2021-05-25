using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_SearchResult_Order
    {
        public int OrderId { get; set; }
        public int OrderTypeId { get; set; }
        public string WayBillNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string CargoCtlNumber { get; set; }
        public string AwbCtnNumber { get; set; }
        public string PickupReferenceNumber { get; set; }
        public string DeliveryReferenceNumber { get; set; }
        public int? ShipperCustomerId { get; set; }
        public string ShipperCustomerName { get; set; }

        public int? ShipperAddressId { get; set; }
        public string ShipperUnitNumber { get; set; }
        public string ShipperAddressLine { get; set; }
        public string ShipperCityName { get; set; }
        public string ShipperProvinceName { get; set; }
        public string ShipperCountryName { get; set; }
        public string ShipperPostCode { get; set; }

        public int? ConsigneeCustomerId { get; set; }
        public string ConsigneeCustomerName { get; set; }

        public int? ConsigneeAddressId { get; set; }
        public string ConsigneeUnitNumber { get; set; }
        public string ConsigneeAddressLine { get; set; }
        public string ConsigneeCityName { get; set; }
        public string ConsigneeProvinceName { get; set; }
        public string ConsigneeCountryName { get; set; }
        public string ConsigneePostCode { get; set; }

        public int BillToCustomerId { get; set; }
        public string BillToCustomerName { get; set; }

        public int? ServiceProviderEmployeeId { get; set; }
        public string ServiceProviderEmployeeName { get; set; }

        public DateTime? ScheduledPickupDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }

        public int? DeliveryOptionId { get; set; }
        public string DeliveryOptionName { get; set; }

        public int? VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; }

        public int UnitTypeId { get; set; }
        public string UnitTypeName { get; set; }

        public int? WeightScaleId { get; set; }
        public string WeightScaleName { get; set; }
        public decimal? WeightTotal { get; set; }
        public int? UnitQuantity { get; set; }
        public int? SkidQuantity { get; set; }
        public int? TotalPiece { get; set; }

        public decimal OrderBasicCost { get; set; }
        public decimal? BasicCostOverriden { get; set; }
        public decimal? FuelSurchargePercentage { get; set; }
        public decimal? DiscountPercentOnOrderCost { get; set; }
        public decimal? ApplicableGstPercent { get; set; }
        public decimal TotalOrderCost { get; set; }
        public decimal? TotalAdditionalServiceCost { get; set; }

        public string OrderedBy { get; set; }
        public string DeliveredBy { get; set; }

        public string BolReferenceNumber { get; set; }
        public string ProReferenceNumber { get; set; }
        public string ShipperName { get; set; }
        public string ShipperAddress { get; set; }
        public decimal? OrderShareAmount { get; set; }
        public bool? IsSharingOnPercent { get; set; }

        public bool IsInvoiced { get; set; }
        public string CommentsForWayBill { get; set; }
        public bool? IsPrintedOnWayBill { get; set; }
        public string CommentsForInvoice { get; set; }
        public bool? IsPrintedOnInvoice { get; set; }

        public bool IsPrePrinted { get; set; }
        public string Remarks { get; set; }
        public string OrderDateString { get; set; }

    }
}
