using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_CustomerEmployeeMapping")]
    public class Lms_CustomerEmployeeMappingPoco : IPoco
    {
        [Key]
        [Column("MappingId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CustomerId { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
