using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_StorageOrder")]
    public class Lms_StorageOrderPoco : IPoco
    {
        [Key]
        [Column("StorageOrderId")]
        public int Id { get; set; }
        public string StorageOrderNumber { get; set; }
        public int CustomerId { get; set; }
        public int? StorageLocationId { get; set; }
        public int BillToCustomerId { get; set; }
        public string DeliverdBy { get; set; }
        public string StorageReferenceNo { get; set; }
        public string CustomerReferenceNo { get; set; }
        public int UnitTypeId { get; set; }
        public int UnitQuantity { get; set; }
        public int WeightScaleId { get; set; }
        public decimal? WeightTotal { get; set; }
        public decimal? RatePerUnit { get; set; }
        public bool? IsFixedRate { get; set; }
        public int? QuantityIn { get; set; }
        public int? QuantityOut { get; set; }
        public int? QuantityRemaining { get; set; }
        public DateTime OrderDate { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
