using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class AddressViewModel
    {
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
