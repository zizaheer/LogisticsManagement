using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeeLoanPayment_Audit")]
    public class Lms_EmployeeLoanPayment_AuditPoco
    {
        [Key]
        public int AutoId { get; set; }
        public Lms_EmployeeLoanPaymentPoco employeeLoanPaymentPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
