using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("App_Province")]
    public class App_ProvincePoco : IPoco
    {
        [Key]
        [Column("ProvinceId")]
        public int Id { get; set; }
        public string ProvinceName { get; set; }
        public string ShortCode { get; set; }
        public int CountryId { get; set; }
        public bool? IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
