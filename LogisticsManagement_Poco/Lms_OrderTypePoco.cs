using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_OrderType")]
    public class Lms_OrderTypePoco : IPoco
    {
        [Key]
        [Column("OrderTypeId")]
        public int Id { get; set; }
        public string OrderTypeName { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
