using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("App_ScreenPermission")]
    public class App_ScreenPermissionPoco :IPoco
    {
        [Key]
        [Column("PermissionId")]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ScreenId { get; set; }
        public bool? IsCreatePermissionAllowed { get; set; }
        public bool? IsReadPermissionAllowed { get; set; }
        public bool? IsUpdatePermissionAllowed { get; set; }
        public bool? IsDeletePermissionAllowed { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
