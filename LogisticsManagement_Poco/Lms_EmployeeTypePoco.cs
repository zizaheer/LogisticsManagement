using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeeType")]
    public class Lms_EmployeeTypePoco : IPoco
    {
        [Key]
        [Column("EmployeeTypeId")]
        public int Id { get; set; }
        public string EmployeeTypeName { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
