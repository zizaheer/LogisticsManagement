using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_StorageOrderDelivery")]
    public class Lms_StorageOrderDeliveryPoco : IPoco
    {
        [Key]
        [Column("StorageDeliveryId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StorageOrderId { get; set; }
        public int? DeliveryQuantity { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveredBy { get; set; }
        public string ReceivedBy { get; set; }
        public string ProofOfDeliveryNote { get; set; }
        public byte[] ProofOfDeliverySignature { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
