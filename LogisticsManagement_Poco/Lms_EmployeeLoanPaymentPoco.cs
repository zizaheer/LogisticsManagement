using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeeLoanPayment")]
    public class Lms_EmployeeLoanPaymentPoco : IPoco
    {
        [Key]
        [Column("PaymentId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal LoanAmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public int TransactionId { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
