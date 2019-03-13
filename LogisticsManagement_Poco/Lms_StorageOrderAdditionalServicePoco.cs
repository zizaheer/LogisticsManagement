using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_StorageOrderAdditionalService")]
    public class Lms_StorageOrderAdditionalServicePoco : IPoco
    {
        [Key, Column("StorageOrderId", Order = 0)]
        public int Id { get; set; }
        [Key, Column(Order = 1)]
        public int AdditionalServiceId { get; set; }

        public decimal ChargeAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
