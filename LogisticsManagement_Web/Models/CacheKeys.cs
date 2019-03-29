using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public static class CacheKeys
    {
        public static List<App_CityPoco> Cities { get; set; }
        public static List<App_CountryPoco> Countries { get; set; }
        public static List<App_ProvincePoco> Provinces { get; set; }
        public static List<Lms_TariffPoco> Tariffs { get; set; }
        public static List<Lms_AddressPoco> Addresses { get; set; }
        public static List<Lms_CustomerPoco> Customers { get; set; }
        public static List<Lms_EmployeePoco> Employees { get; set; }
    }
}
