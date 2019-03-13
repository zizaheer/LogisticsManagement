using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_EmployeeVehicleMapping")]
    public class Lms_EmployeeVehicleMappingPoco : IPoco
    {
        [Key]
        [Column("MappingId")]
        public int Id { get; set; }  
        public int EmployeeId { get; set; }  
        public int VehicleTypeId { get; set; }  
        public string VehicleNumber { get; set; }  
        public string Remarks { get; set; }  
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
