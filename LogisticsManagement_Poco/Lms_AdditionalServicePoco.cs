using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_AdditionalService")]
    public class Lms_AdditionalServicePoco : IPoco
    {
        [Key]
        [Column("ServiceId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public bool? IsPriceApplicable { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool IsTaxApplicable { get; set; }
        public bool PayToDriver { get; set; }
        public bool IsApplicableForStorage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
