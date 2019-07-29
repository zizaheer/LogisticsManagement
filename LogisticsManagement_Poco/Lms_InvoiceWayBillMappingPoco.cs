using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_InvoiceWayBillMapping")]
    public class Lms_InvoiceWayBillMappingPoco :IPoco
    {
        [Key]
        [Column("MappingId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string WayBillNumber { get; set; }
        //public int OrderId { get; set; }
        public decimal TotalWayBillAmount { get; set; }
        public bool IsClear { get; set; }
    }
}
