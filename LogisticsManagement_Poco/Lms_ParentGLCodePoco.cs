using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_ParentGLCode")]
    public class Lms_ParentGLCodePoco : IPoco
    {
        [Key, Column("GLCode")]
        public int Id { get; set; }

        public int? ParentCode { get; set; }
        public int ParentAccountTypeId { get; set; }
        public string CodePurpose { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
