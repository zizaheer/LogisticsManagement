using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_ChartOfAccount")]
    public class Lms_ChartOfAccountPoco : IPoco
    {
        [Key]
        [Column("AccountId")]
        public int Id { get; set; }

        public int ParentGLCode { get; set; }
        public string AccountName { get; set; }
        public int BranchId { get; set; }
        public decimal? CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
