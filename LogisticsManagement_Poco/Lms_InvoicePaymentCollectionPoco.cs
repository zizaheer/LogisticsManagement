using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_InvoicePaymentCollection")]
    public class Lms_InvoicePaymentCollectionPoco : IPoco
    {
        [Key]
        [Column("PaymentCollectionId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        public decimal PaymentAmount { get; set; }
        public int PaymentMethodId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int TransactionId { get; set; }
        public decimal? CashAmount { get; set; }
        public decimal? ChequeAmount { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public int? BankId { get; set; }
        public string Remarks { get; set; }
    }
}



