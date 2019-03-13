using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("App_Screen")]
    public  class App_ScreenPoco : IPoco
    {
        [Key]
        [Column("ScreenId")]
        public int Id { get; set; }
        public string ScreenName { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
