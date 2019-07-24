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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderTypeId { get; set; }
        public string WayBillNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string CargoCtlNumber { get; set; }
        public string AwbCtnNumber { get; set; }
        public string PickupReferenceNumber { get; set; }
        public int? ShipperCustomerId { get; set; }
        public int? ShipperAddressId { get; set; }
        public int? ConsigneeCustomerId { get; set; }
        public int? ConsigneeAddressId { get; set; }
        public int BillToCustomerId { get; set; }
        public int? ServiceProviderEmployeeId { get; set; }

        public DateTime? ScheduledPickupDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int? CityId { get; set; }
        public int? DeliveryOptionId { get; set; }
        public int? VehicleTypeId { get; set; }
        public int UnitTypeId { get; set; }
        public int? WeightScaleId { get; set; }
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
        public string DepartmentName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhoneNumber { get; set; }

        public string DeliveredBy { get; set; }
        public string BolReferenceNumber { get; set; }
        public string ProReferenceNumber { get; set; }
        public string ShipperName { get; set; }
        public string ShipperAddress { get; set; }

        public bool IsInvoiced { get; set; }
        public string CommentsForWayBill { get; set; }
        public bool? IsPrintedOnWayBill { get; set; }
        public string CommentsForInvoice { get; set; }
        public bool? IsPrintedOnInvoice { get; set; }

        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

        public List<Lms_OrderStatusPoco> Lms_OrderStatusPocos { get; set; }
    }
}
