using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class DispatchBoardViewModel
    {
        public List<Lms_EmployeePoco> Employees { get; set; }
        public Lms_ConfigurationPoco Configuration { get; set; }
        public List<DispatchedOrderViewModel> DispatchedOrderViewModels { get; set; }
    }
}
