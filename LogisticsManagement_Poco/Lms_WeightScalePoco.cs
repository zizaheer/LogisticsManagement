using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_WeightScale")]
    public  class Lms_WeightScalePoco : IPoco
    {
        [Key]
        [Column("WeightScaleId")]
        public int Id { get; set; }
        public string ScaleName { get; set; }
        public string ShortCode { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
