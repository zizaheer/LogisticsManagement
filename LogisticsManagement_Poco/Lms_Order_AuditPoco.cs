using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Order_Audit")]
    public class Lms_Order_AuditPoco
    {
        public Lms_OrderPoco orderPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
