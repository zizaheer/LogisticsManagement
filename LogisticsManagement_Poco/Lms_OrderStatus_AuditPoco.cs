using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_OrderStatus_Audit")]
    public class Lms_OrderStatus_AuditPoco
    {
        public Lms_OrderStatusPoco orderStatusPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
