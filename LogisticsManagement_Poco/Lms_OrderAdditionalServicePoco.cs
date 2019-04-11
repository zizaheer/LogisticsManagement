using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_OrderAdditionalService")]
    public class Lms_OrderAdditionalServicePoco : IPoco
    {
        [Key]
        [Column("OrderAdditionalServiceId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int AdditionalServiceId { get; set; }
        public decimal AdditionalServiceFee { get; set; }
        public decimal? DriverPercentageOnAddService { get; set; }
        public bool? IsTaxAppliedOnAddionalService { get; set; }
        public decimal? TaxAmountOnAdditionalService { get; set; }
    }
}
