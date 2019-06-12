using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_CustomerAddressMapping")]
    public class Lms_CustomerAddressMappingPoco : IPoco
    {
        [Key]
        [Column("MappingId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public byte AddressTypeId { get; set; }
        public bool IsDefault { get; set; }
    }
}
