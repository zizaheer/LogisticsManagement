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

        public string ParentGLForCustomerAccount { get; set; }
        public string ParentGLForEmployeeAccount { get; set; }
        public string SalesTaxPayableAccount { get; set; }
        public string SalesIncomeAccount { get; set; }
        public string SalaryExpenseAccount { get; set; }
        public string BonusExpenseAccount { get; set; }
        public string OtherReceivableAccount { get; set; }
        public string OtherPayableAccount { get; set; }
        public string OtherIncomeAccount { get; set; }
        public string OtherExpenseAccount { get; set; }

        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
