using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Bank")]
    public class Lms_BankPoco : IPoco
    {
        [Key]
        [Column("BankId")]
        public int Id { get; set; }
        public string InstituteNumber { get; set; }
        public string BankName { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
