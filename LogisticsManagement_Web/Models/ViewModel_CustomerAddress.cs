using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_CustomerAddress
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int AddressId { get; set; }
        public int AddressTypeId { get; set; }
        public string AddressTypeName { get; set; }
        public bool IsDefault { get; set; }

        public string UnitNumber { get; set; }
        public string HouseNumber { get; set; }
        public string AddressLine { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string PostCode { get; set; }
        public string MergedAddressLine { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string ContactPerson { get; set; }

        public List<App_CityPoco> Cities { get; set; }
        public List<App_ProvincePoco> Provinces { get; set; }
        public List<App_CountryPoco> Countries { get; set; }
    }
}
