using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_AddressMapping")]
    public class Lms_AddressMappingPoco : IPoco
    {
        [Key]
        [Column("MappingId")]
        public int Id { get; set; }
        public int AddressId { get; set; }
        public int? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public int AddressTypeId { get; set; }
        public bool IsDefaultAddress { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
