using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_Poco
{
    public class Lms_Customer_AuditPoco
    {
        public Lms_CustomerPoco customerPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
