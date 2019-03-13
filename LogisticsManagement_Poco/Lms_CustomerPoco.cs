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
        public int Id { get; set; }
        public int CustomerNumber { get; set; }
        public int AccountId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public string ContactPersonNumber { get; set; }
        public bool IsGstApplicable { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public Byte InvoiceDueDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
