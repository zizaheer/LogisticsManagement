﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Address")]
    public class Lms_AddressPoco : IPoco
    {
        [Key]
        [Column("AddressId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UnitNumber { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNumber { get; set; }
        public string AddressLine { get; set; }
        public int CityId { get; set; }
        public int ProvinceId { get; set; }
        public int CountryId { get; set; }
        public string PostCode { get; set; }
        public string ContactPersonName { get; set; }
        public string Fax { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string PhoneNumber2 { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
