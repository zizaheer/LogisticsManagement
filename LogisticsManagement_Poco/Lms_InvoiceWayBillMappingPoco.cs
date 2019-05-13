using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_InvoiceWayBillMapping")]
    public class Lms_InvoiceWayBillMappingPoco :IPoco
    {
        [Key]
        [Column("MappingId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string WayBillNumber { get; set; }
        //public int OrderId { get; set; }
        public decimal TotalWayBillAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? WaivedAmount { get; set; }
        public int? PaymentMethodId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int TransactionId { get; set; }
        public decimal? CashAmount { get; set; }
        public decimal? ChequeAmount { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public int? BankId { get; set; }
        public string Remarks { get; set; }
    }
}
