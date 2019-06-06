using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_Address
    {
        public List<Lms_AddressPoco> Addresses { get; set; }
        public List<App_CityPoco> Cities { get; set; }
        public List<App_ProvincePoco> Provinces { get; set; }
        public List<App_CountryPoco> Countries { get; set; }
        public List<ViewModel_AddressForAutoComplete> AddressLinesForAutoComplete { get; set; }

    }

    
}
