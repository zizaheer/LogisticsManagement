using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Invoice_Audit")]
    public class Lms_Invoice_AuditPoco
    {
        public Lms_BillPaymentPoco billPaymentPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
