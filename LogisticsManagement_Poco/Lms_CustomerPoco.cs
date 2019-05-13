using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Customer")]
    public class Lms_CustomerPoco : IPoco
    {
        [Key]
        [Column("CustomerId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string CustomerName { get; set; }
        
        public bool IsGstApplicable { get; set; }
        public decimal? FuelSurChargePercentage { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public byte? InvoiceDueDays { get; set; }

        public int? BillingAddressId { get; set; }
        public int? MailingAddressId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
