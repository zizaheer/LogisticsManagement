﻿using System;
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PayrollGenerationId { get; set; }
        public int EmployeeId { get; set; }

        public string WaybillNo { get; set; }
        public decimal? FuelPercent { get; set; }
    }
}
