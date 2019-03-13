using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("App_LoginHistory")]
    public class App_LoginHistoryPoco : IPoco
    {
        public int Id { get; set; }
        public int LoginHistoryId { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public string LoginIP { get; set; }
        public string LoginLocation { get; set; }
        public string LoginBrowser { get; set; }
        public DateTime LogoutTime { get; set; }
    }
}
