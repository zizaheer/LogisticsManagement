using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Employee")]
    public class Lms_EmployeePoco : IPoco
    {
        [Key]
        [Column("EmployeeId")]
        public int Id { get; set; }
        public string EmployeeNumber { get; set; }
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DriverLicenseNo { get; set; }
        public string SocialInsuranceNo { get; set; }
        public string UnitNumber { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNumber { get; set; }
        public string AddressLine { get; set; }
        public int? CityId { get; set; }
        public int? ProvinceId { get; set; }
        public int? CountryId { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EmailAddress { get; set; }
        public int EmployeeTypeId { get; set; }
        public bool? IsHourlyPaid { get; set; }
        public decimal? HourlyRate { get; set; }
        public bool? IsSalaried { get; set; }
        public decimal? SalaryAmount { get; set; }
        public byte? SalaryTerm { get; set; }
        public bool? IsCommissionProvided { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public bool? IsFuelChargeProvided { get; set; }
        public decimal? FuelPercentage { get; set; }
        public decimal? RadioInsuranceAmount { get; set; }
        public decimal? InsuranceAmount { get; set; }
        public int? TermDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
