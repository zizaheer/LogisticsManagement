using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Configuration")]
    public class Lms_ConfigurationPoco : IPoco
    {
        [Key]
        [Column("ConfigurationId")]
        public int Id { get; set; }
        public string TaxToCall { get; set; }
        public decimal? TaxAmount { get; set; }
        public bool? IsSignInRequiredForDispatch { get; set; }
        public string WayBillPrefix { get; set; }
        public string DeliveryWBNoStartFrom { get; set; }
        public string MiscWBNoStartFrom { get; set; }
        public string StorageWBNoStartFrom { get; set; }
        public string InvoiceNumberStartFrom { get; set; }
        public int? DefaultWeightScaleId { get; set; }
        public decimal? DefaultFuelSurcharge { get; set; }

        public int? ParentGLForCustomerAccount { get; set; }
        public int? ParentGLForEmployeeAccount { get; set; }
        public int? SalesTaxPayableAccount { get; set; }
        public int? SalesIncomeAccount { get; set; }
        public int? SalaryExpenseAccount { get; set; }
        public int? BonusExpenseAccount { get; set; }
        public int? OtherReceivableAccount { get; set; }
        public int? OtherPayableAccount { get; set; }
        public int? OtherIncomeAccount { get; set; }
        public int? OtherExpenseAccount { get; set; }

        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
