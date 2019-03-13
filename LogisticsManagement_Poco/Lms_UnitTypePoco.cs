using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_UnitType")]
    public class Lms_UnitTypePoco : IPoco
    {
        [Key]
        [Column("UnitTypeId")]
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string ShortCode { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
