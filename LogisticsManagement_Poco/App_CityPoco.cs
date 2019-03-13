using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("App_City")]
    public class App_CityPoco : IPoco
    {
        [Key]
        [Column("CityId")]
        public int Id { get; set; }
        public string CityName { get; set; }
        public int ProvinceId { get; set; }
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
