using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_PaymentMethod")]
    public class Lms_PaymentMethodPoco : IPoco
    {
        [Key]
        [Column("PaymentMethodId")]
        public int Id { get; set; }
        public string MethodName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
