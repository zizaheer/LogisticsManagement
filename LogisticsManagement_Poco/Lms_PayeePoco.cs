using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Payee")]
    public class Lms_PayeePoco : IPoco
    {
        [Key]
        [Column("PayeeId")]
        public int Id { get; set; }
        public string AccountNo { get; set; }
        public string PayeeName { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
