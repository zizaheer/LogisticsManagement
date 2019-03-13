using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_CompanyInfo")]
    public class Lms_CompanyInfoPoco : IPoco
    {
        [Key]
        [Column("CompanyId")]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string MainAddress { get; set; }
        public string PostCode { get; set; }
        public int CityId { get; set; }
        public int? ProvinceId { get; set; }
        public int CountryId { get; set; }
        public string TaxNumber { get; set; }
        public string CompanyRegistrationNo { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

    }
}
