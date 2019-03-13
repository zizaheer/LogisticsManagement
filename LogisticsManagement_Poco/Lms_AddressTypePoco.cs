using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_AddressType")]
    public class Lms_AddressTypePoco: IPoco
    {
        [Key]
        [Column("AddressTypeId")]
        public int Id { get; set; }
        public string TypeName { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
