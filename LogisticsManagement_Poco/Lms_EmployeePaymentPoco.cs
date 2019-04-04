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
        public int PayrollGenerationId { get; set; }
        public int EmployeeId { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal? LoanDeduction { get; set; }
        public decimal? InsuranceDeduction { get; set; }
        public decimal? RadioInsuranceDeduction { get; set; }
        public decimal? OtherDeduction { get; set; }
        public decimal? BonusAmount { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
