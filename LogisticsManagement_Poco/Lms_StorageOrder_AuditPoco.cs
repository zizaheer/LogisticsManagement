﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_StorageOrder_Audit")]
    public class Lms_StorageOrder_AuditPoco
    {
        public Lms_StorageOrderPoco storageOrderPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
