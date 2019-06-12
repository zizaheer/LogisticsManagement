using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    /// <summary>
    /// This class is only used to map Json object received from 'Customer Address Entry' screen for Add/Update/Remove addresses for customers
    /// </summary>
    public class CustomerAddressMapping
    {
        public int CustomerId { get; set; }
        public byte AddressTypeId { get; set; }
        public int AddressId { get; set; }
        public bool IsDefault { get; set; }
        public string AddressLine { get; set; }
        public string UnitNumber { get; set; }
        public int CityId { get; set; }
        public int ProvinceId { get; set; }
        public int CountryId { get; set; }
        public string PostCode { get; set; }
        public string ContactPersonName { get; set; }
        public string EmailAddress1 { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string Fax { get; set; }

    }
}
