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
        [Key]
        [Column("TransactionId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TransactionNo { get; set; }
        public string DebitAccountNumber { get; set; }
        public string CreditAccountNumber { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public string Remarks { get; set; }
    }
}
