﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeePayment_Audit")]
    public class Lms_EmployeePayment_AuditPoco
    {
        public Lms_EmployeePaymentPoco employeePaymentPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
