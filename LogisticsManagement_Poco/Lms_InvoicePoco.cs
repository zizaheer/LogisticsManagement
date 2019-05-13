using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Invoice")]
    public class Lms_InvoicePoco : IPoco
    {
        [Key]
        [Column("InvoiceId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BillerCustomerId { get; set; }
        public string BillerDepartment { get; set; }
        public string WaybillNumbers { get; set; }
        public decimal? TotalInvoiceAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public int TransactionId { get; set; }
        public int PrintCount { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
