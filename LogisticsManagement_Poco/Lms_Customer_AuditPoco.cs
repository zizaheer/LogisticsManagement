using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LogisticsManagement_Poco
{
    public class Lms_Customer_AuditPoco
    {
        [Key]
        public int AutoId { get; set; }
        public Lms_CustomerPoco customerPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
