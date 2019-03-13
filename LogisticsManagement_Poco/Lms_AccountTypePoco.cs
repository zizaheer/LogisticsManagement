using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_AccountType")]
    public class Lms_AccountTypePoco : IPoco
    {
        [Key]
        [Column("AccountTypeId")]
        public int Id { get; set; }
        public string TypeName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

    }
}
