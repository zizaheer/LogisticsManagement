using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_AccountBalance")]
    public class Lms_AccountBalancePoco : IPoco
    {
        [Key]
        [Column("AccountId")]
        public int Id { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime BalanceLastUpdated { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
