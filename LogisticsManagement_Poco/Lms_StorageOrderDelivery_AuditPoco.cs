using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_StorageOrderDelivery_Audit")]
    public class Lms_StorageOrderDelivery_AuditPoco
    {
        public Lms_StorageOrderDeliveryPoco storageOrderDeliveryPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
