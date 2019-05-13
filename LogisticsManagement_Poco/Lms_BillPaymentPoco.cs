using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_BillPayment")]
    public class Lms_BillPaymentPoco : IPoco
    {
        [Key]
        [Column("PaymentId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PayeeId { get; set; }
        public decimal PaidAmount { get; set; }
        public int TransactionId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal? CashAmount { get; set; }
        public decimal? ChequeAmount { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public int BankId { get; set; }
        public int Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

    }
}
