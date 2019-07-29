using LogisticsManagement_Poco;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_DataAccess
{
    public class LogisticsContext : DbContext
    {
        public LogisticsContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<App_CityPoco> appCityPocos { get; set; }
        public DbSet<App_CountryPoco> appCountryPocos { get; set; }
        public DbSet<App_DocumentCategoryPoco> appDocumentCategoryPocos { get; set; }
        public DbSet<App_DocumentTypePoco> appDocumentTypePocos { get; set; }
        public DbSet<App_LoginHistoryPoco> appLoginHistoryPocos { get; set; }
        public DbSet<App_ProvincePoco> appProvincePocos { get; set; }
        public DbSet<App_ScreenPoco> appScreenPocos { get; set; }
        public DbSet<App_ScreenPermissionPoco> appScreenPermissionPocos { get; set; }
        public DbSet<App_UserPoco> appUserPocos { get; set; }
        public DbSet<App_UserGroupPoco> appUserGroupPocos { get; set; }
        public DbSet<Lms_ParentGLCodePoco> lmsParentGLCodePocos { get; set; }
        public DbSet<Lms_AccountTypePoco> lmsAccountTypePocos { get; set; }
        public DbSet<Lms_AdditionalServicePoco> lmsAdditionalServicePocos { get; set; }
        public DbSet<Lms_AddressPoco> lmsAddressPocos { get; set; }
        public DbSet<Lms_AddressTypePoco> lmsAddressTypePocos { get; set; }
        public DbSet<Lms_BankPoco> lmsBankPocos { get; set; }
        public DbSet<Lms_BillPaymentPoco> lmsBillPaymentPocos { get; set; }
        public DbSet<Lms_ChartOfAccountPoco> lmsChartOfAccountPocos { get; set; }
        public DbSet<Lms_CompanyBranchInfoPoco> lmsCompanyBranchInfoPocos { get; set; }
        public DbSet<Lms_CompanyInfoPoco> lmsCompanyInfoPocos { get; set; }
        public DbSet<Lms_ConfigurationPoco> lmsConfigurationPocos { get; set; }
        public DbSet<Lms_CustomerPoco> lmsCustomerPocos { get; set; }
        public DbSet<Lms_CustomerAddressMappingPoco> lmsCustomerAddressMappingPocos { get; set; }
        public DbSet<Lms_CustomerEmployeeMappingPoco> lmsCustomerEmployeeMappingPocos { get; set; }
        public DbSet<Lms_DeliveryOptionPoco> lmsDeliveryOptionPocos { get; set; }
        public DbSet<Lms_EmployeePoco> lmsEmployeePocos { get; set; }
        public DbSet<Lms_EmployeeLoanPoco> lmsEmployeeLoanPocos { get; set; }
        public DbSet<Lms_EmployeeLoanPaymentPoco> lmsEmployeeLoanPaymentPocos { get; set; }
        public DbSet<Lms_EmployeePaymentPoco> lmsEmployeePaymentPocos { get; set; }
        public DbSet<Lms_EmployeePayrollPoco> lmsEmployeePayrollPocos { get; set; }
        public DbSet<Lms_EmployeePayrollDetailPoco> lmsEmployeePayrollDetailPocos { get; set; }
        public DbSet<Lms_EmployeeTimesheetPoco> lmsEmployeeTimesheetPocos { get; set; }
        public DbSet<Lms_EmployeeTypePoco> lmsEmployeeTypePocos { get; set; }
        public DbSet<Lms_EmployeeVehicleMappingPoco> lmsEmployeeVehicleMappingPocos { get; set; }
        public DbSet<Lms_InvoicePoco> lmsInvoicePocos { get; set; }
        public DbSet<Lms_InvoicePaymentCollectionPoco> lmsInvoicePaymentCollectionPocos { get; set; }
        public DbSet<Lms_InvoiceStorageOrderMappingPoco> lmsInvoiceStorageOrderMappingPocos { get; set; }
        public DbSet<Lms_InvoiceWayBillMappingPoco> lmsInvoiceWayBillMappingPocos { get; set; }
        public DbSet<Lms_OrderPoco> lmsOrderPocos { get; set; }
        public DbSet<Lms_OrderDocumentPoco> lmsOrderDocumentPocos { get; set; }
        public DbSet<Lms_OrderStatusPoco> lmsOrderStatusPocos { get; set; }
        public DbSet<Lms_OrderTypePoco> lmsOrderTypePocos { get; set; }
        public DbSet<Lms_OrderAdditionalServicePoco> _orderAdditionalServicePocos { get; set; }
        public DbSet<Lms_PayeePoco> lmsPayeePocos { get; set; }
        public DbSet<Lms_PaymentMethodPoco> lmsPaymentMethodPocos { get; set; }
        public DbSet<Lms_StorageOrderPoco> lmsStorageOrderPoco { get; set; }
        public DbSet<Lms_StorageOrderAdditionalServicePoco> lmsStorageOrderAdditionalServicePocos { get; set; }
        public DbSet<Lms_StorageOrderDeliveryPoco> lmsStorageOrderDeliveryPocos { get; set; }
        public DbSet<Lms_TariffPoco> lmsTariffPocos { get; set; }
        public DbSet<Lms_TransactionPoco> lmsTransactionPocos { get; set; }
        public DbSet<Lms_TransactionDetailPoco> lmsTransactionDetailPocos { get; set; }
        public DbSet<Lms_UnitTypePoco> lmsUnitTypePocos { get; set; }

        public DbSet<Lms_VehicleTypePoco> lmsVehicleTypePocos { get; set; }
        public DbSet<Lms_VehicleUnitMappingPoco> lmsVehicleUnitMappingPocos { get; set; }
        public DbSet<Lms_WeightScalePoco> lmsWeightScalePocos { get; set; }

        public DbSet<Lms_BillPayment_AuditPoco> lmsBillPayment_AuditPocos { get; set; }
        public DbSet<Lms_Customer_AuditPoco> lmsCustomer_AuditPocos { get; set; }
        public DbSet<Lms_Employee_AuditPoco> lmsEmployee_AuditPocos { get; set; }
        public DbSet<Lms_EmployeeLoan_AuditPoco> lmsEmployeeLoan_AuditPocos { get; set; }
        public DbSet<Lms_EmployeeLoanPayment_AuditPoco> lmsEmployeeLoanPayment_AuditPocos { get; set; }
        public DbSet<Lms_EmployeePayment_AuditPoco> lmsEmployeePayment_AuditPocos { get; set; }
        public DbSet<Lms_EmployeeTimesheet_AuditPoco> lmsEmployeeTimesheet_AuditPocos { get; set; }
        public DbSet<Lms_Invoice_AuditPoco> lmsInvoice_AuditPocos { get; set; }
        public DbSet<Lms_InvoiceStorageOrderMapping_AuditPoco> lmsInvoiceStorageOrderMapping_AuditPocos { get; set; }
        public DbSet<Lms_InvoiceWayBillMapping_AuditPoco> lmsInvoiceWayBillMapping_AuditPocos { get; set; }
        public DbSet<Lms_Order_AuditPoco> lmsOrder_AuditPocos { get; set; }
        public DbSet<Lms_OrderStatus_AuditPoco> lmsOrderStatus_AuditPocos { get; set; }
        public DbSet<Lms_StorageOrder_AuditPoco> lmsStorageOrder_AuditPocos { get; set; }
        public DbSet<Lms_StorageOrderDelivery_AuditPoco> lmsStorageOrderDelivery_AuditPocos { get; set; }

        public DbQuery<Lms_StoredProcedureResult> returnedValueString { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lms_TransactionPoco>().HasKey(table => new { table.Id, table.SerialNo });
            modelBuilder.Entity<Lms_StorageOrderAdditionalServicePoco>().HasKey(table => new { table.Id, table.AdditionalServiceId });

        }

    }
}
