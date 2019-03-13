using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeePayrollDetail")]
    public class Lms_EmployeePayrollDetailPoco : IPoco
    {
        [Key]
        [Column("DetailId")]
        public int Id { get; set; }
        public int PayrollGenerationId { get; set; }
        public int EmployeeId { get; set; }
        public bool? DoesTaxDeductionApply { get; set; }
        public decimal? TaxDeductionPercent { get; set; }
        public bool? DoesTaxAdditionApply { get; set; }
        public decimal? TaxAdditionPercent { get; set; }
        public decimal? TotalEarning { get; set; }
    }
}
