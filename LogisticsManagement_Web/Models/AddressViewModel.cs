using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class AddressViewModel
    {
        public List<Lms_AddressPoco> Addresses { get; set; }
        public List<App_CityPoco> Cities { get; set; }
        public List<App_ProvincePoco> Provinces { get; set; }
        public List<App_CountryPoco> Countries { get; set; }
    }

    public class AddressViewModelForDataTable
    {
        public int AddressId { get; set; }
        public string UnitNumber { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNumber { get; set; }
        public string AddressLine { get; set; }
        public string CityName { get; set; }
        public string ProvinceShortCode { get; set; }
        public string CountryName { get; set; }
        public string PostCode { get; set; }
        public string ContactPersonName { get; set; }
        public string Fax { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string PhoneNumber2 { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
    }


    public class AddressViewModelForAutoComplete
    {
        // This 2 fields (label, value) are required to pass on lower case for Jquiry UI Autocomplete to work
        // Give some more thoughts later
        public string label { get; set; } 
        public string value { get; set; }

        public string Id { get; set; }
        public string UnitNumber { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNumber { get; set; }
        public string AddressLine { get; set; }
        public string CityId { get; set; }
        public string ProvinceId { get; set; }
        public string CountryId { get; set; }
        public string PostCode { get; set; }
        public string ContactPersonName { get; set; }
        public string Fax { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string PhoneNumber2 { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
    }
}
