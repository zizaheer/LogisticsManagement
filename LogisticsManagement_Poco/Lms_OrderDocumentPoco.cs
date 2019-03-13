using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_OrderDocument")]
    public class Lms_OrderDocumentPoco : IPoco
    {
        [Key]
        [Column("DocumentId")]
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public string WayBillNumber { get; set; }
        public byte[] DocumentContent { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
