using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Order")]
    public class Lms_OrderPoco :IPoco
    {
        [Key]
        [Column("OrderId")]
        public int Id { get; set; }
        public int OrderTypeId { get; set; }
        public string WayBillNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string CargoCtlNumber { get; set; }
        public string AwbCtnNumber { get; set; }
        public int? ShipperCustomerId { get; set; }
        public int? ConsigneeCustomerId { get; set; }
        public int BillToCustomerId { get; set; }
        public DateTime? ScheduledPickupDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int? CityId { get; set; }
        public int? DeliveryOptionId { get; set; }
        public int? VehicleTypeId { get; set; }
        public int UnitTypeId { get; set; }
        public int? WeightScaleId { get; set; }
        public decimal? WeightTotal { get; set; }
        public int UnitQuantity { get; set; }
        public decimal OrderCost { get; set; }
        public decimal? DiscountPercentOnOrderCost { get; set; }
        public decimal? OverriddenOrderCost { get; set; }
        public bool IsTaxAppliedOnOrderCost { get; set; }
        public decimal? TaxAmountOnOrderCost { get; set; }
        public int AdditionalServiceId { get; set; }
        public decimal? AdditionalServiceFee { get; set; }
        public decimal? DriverPercentageOnAddService { get; set; }
        public bool? IsTaxAppliedOnAddionalService { get; set; }
        public decimal? TaxAmountOnAdditionalService { get; set; }
        public decimal? FuelSurchargePercentage { get; set; }
        public decimal? FuelSurchargeDiscountPercent { get; set; }
        public bool? IsTaxAppliedOnFuelSurcharge { get; set; }
        public decimal? TaxAmountOnFuelSurcharge { get; set; }
        public string OrderedBy { get; set; }
        public string DepartmentName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

        public List<Lms_OrderStatusPoco> orderStatusPocos { get; set; }
    }
}
