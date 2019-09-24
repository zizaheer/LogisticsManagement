using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_City
    {
        //public int CityId { get; set; }
        //public string CityName { get; set; }
        //public int ProvinceId { get; set; }
        //public string ProvinceName { get; set; }
        //public int CountryId { get; set; }
        //public string CountryName { get; set; }

        public List<App_CityPoco> Cities { get; set; }
        public List<App_ProvincePoco> Provinces { get; set; }
        public List<App_CountryPoco> Countries { get; set; }

    }
}
