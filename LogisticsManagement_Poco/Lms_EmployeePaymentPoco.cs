using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeePayment")]
    public class Lms_EmployeePaymentPoco : IPoco
    {
        [Key]
        [Column("PaymentId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string PaymentReferenceNo { get; set; }
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
