using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeeLoan_Audit")]
    public class Lms_EmployeeLoan_AuditPoco
    {
        public Lms_EmployeeLoanPoco employeeLoanPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
