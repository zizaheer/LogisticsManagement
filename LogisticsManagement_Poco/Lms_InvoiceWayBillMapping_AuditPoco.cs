﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_InvoiceWayBillMapping_Audit")]
    public class Lms_InvoiceWayBillMapping_AuditPoco
    {
        [Key]
        public int AutoId { get; set; }
        public Lms_InvoiceWayBillMappingPoco invoiceWayBillMappingPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
