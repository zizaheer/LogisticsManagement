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
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal LoanAmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionNo { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
