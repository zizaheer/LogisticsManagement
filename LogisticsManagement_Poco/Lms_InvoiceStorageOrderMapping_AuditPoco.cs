using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_InvoiceStorageOrderMapping_Audit")]
    public class Lms_InvoiceStorageOrderMapping_AuditPoco
    {
        public Lms_InvoiceStorageOrderMappingPoco invoiceStorageOrderMappingPoco { get; set; }
        public string changeStatusFlag { get; set; }
    }
}
