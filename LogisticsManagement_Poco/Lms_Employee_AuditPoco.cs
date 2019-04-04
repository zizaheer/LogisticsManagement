using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Employee_Audit")]
    public class Lms_Employee_AuditPoco : IPoco
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AutoId")]
        public int Id { get; set; }
        public Lms_EmployeePoco employeePoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
