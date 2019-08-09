using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeePayroll")]
    public class Lms_EmployeePayrollPoco :IPoco
    {
        [Key]
        [Column("PayrollGenerationId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmployeeTypeId { get; set; }
        public int EmployeeId { get; set; }
        public decimal? PayrollAmount { get; set; }
        public decimal? BonusAmount { get; set; }
        public decimal? LoanDeduction { get; set; }
        public decimal? InsuranceDeduction { get; set; }
        public decimal? RadioInsuranceDeduction { get; set; }
        public decimal? OtherDeduction { get; set; }
        public decimal? DeductTaxPercent { get; set; }
        public decimal? PayTaxPercent { get; set; }
        public decimal TotalEarning { get; set; }
        public int TransactionId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
