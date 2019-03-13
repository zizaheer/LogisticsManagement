using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("App_DocumentCategory")]
    public class App_DocumentCategoryPoco : IPoco
    {
        [Key]
        [Column("CategoryId")]
        public int Id { get; set; }
        public int CategoryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
