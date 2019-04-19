using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class DispatchBoardViewModel
    {
        public List<Lms_CustomerPoco> Customers { get; set; }
        //public List<Lms_AddressPoco> Addressess { get; set; }
        //public List<App_CityPoco> Cities { get; set; }
        //public List<App_ProvincePoco> Provinces { get; set; }
        //public List<App_CountryPoco> Countries { get; set; }
        public List<Lms_EmployeePoco> Employees { get; set; }
        public List<Lms_EmployeeTimesheetPoco> EmployeesTimesheet { get; set; }
        public List<Lms_OrderPoco> Orders { get; set; }

        public Lms_ConfigurationPoco Configuration { get; set; }
    }
}
