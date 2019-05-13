using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_AccTransactionDetail")]
    public class Lms_AccTransactionDetailPoco : IPoco
    {
        [Key, Column("AccountId", Order = 0)]
        public int Id { get; set; }
        [Key, Column(Order = 1)]
        public int TransactionId { get; set; }

        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal CumulativeBalance { get; set; }
    }
}
