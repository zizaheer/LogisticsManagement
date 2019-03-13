using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_VehicleUnitMapping")]
    public class Lms_VehicleUnitMappingPoco : IPoco
    {
        [Key]
        [Column("MappingId")]
        public int Id { get; set; }
        public int VehicleTypeId { get; set; }
        public int UnitTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
