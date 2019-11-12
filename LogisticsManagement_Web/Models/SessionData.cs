using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class SessionData
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int? BranchId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public int? ProvinceId { get; set; }
        public int? CountryId { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] ProfilePicture { get; set; }

        public int? LoggedInEmployeeId { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyTelephone { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyTaxNumber { get; set; }

    }
}
