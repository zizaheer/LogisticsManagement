using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_DeliveryOption")]
    public class Lms_DeliveryOptionPoco :IPoco
    {
        [Key]
        [Column("DeliveryOptionId")]
        public int Id { get; set; }
        public string OptionName { get; set; }
        public string ShortCode { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
