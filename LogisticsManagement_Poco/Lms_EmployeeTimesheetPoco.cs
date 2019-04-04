using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeeTimesheet")]
    public class Lms_EmployeeTimesheetPoco : IPoco
    {
        [Key]
        [Column("TimesheetId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime DateWorked { get; set; }
        public DateTime SignInDatetime { get; set; }
        public DateTime SignOutDatetime { get; set; }
        public decimal BreakTime { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

       
    }
}
