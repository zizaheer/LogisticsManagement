using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Transaction")]
    public class Lms_TransactionPoco : IPoco
    {
        [Key, Column("TransactionId", Order =0)]
        public int Id { get; set; }
        [Key, Column(Order = 1)]
        public int SerialNo { get; set; }

        public int AccountId { get; set; }
        public decimal TransactionAmount { get; set; }
        public string Remarks { get; set; }
    }
}
