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

        DbSet<App_CityPoco> appCityPocos { get; set; }
        DbSet<App_CountryPoco> appCountryPocos { get; set; }
        DbSet<App_DocumentCategoryPoco> appDocumentCategoryPocos { get; set; }
        DbSet<App_DocumentTypePoco> appDocumentTypePocos { get; set; }
        DbSet<App_LoginHistoryPoco> appLoginHistoryPocos { get; set; }
        DbSet<App_ProvincePoco> appProvincePocos { get; set; }
        DbSet<App_ScreenPoco> appScreenPocos { get; set; }
        DbSet<App_ScreenPermissionPoco> appScreenPermissionPocos { get; set; }
        DbSet<App_UserPoco> appUserPocos { get; set; }
        DbSet<App_UserGroupPoco> appUserGroupPocos { get; set; }
        DbSet<Lms_AccountBalancePoco> lmsAccountBalancePocos { get; set; }
        DbSet<Lms_AccountTypePoco> lmsAccountTypePocos { get; set; }
        DbSet<Lms_AccTransactionDetailPoco> lmsAccTransactionDetailPocos { get; set; }
        DbSet<Lms_AdditionalServicePoco> lmsAdditionalServicePocos { get; set; }
        DbSet<Lms_AddressPoco> lmsAddressPocos { get; set; }
        DbSet<Lms_AddressMappingPoco> lmsAddressMappingPocos { get; set; }
        DbSet<Lms_AddressTypePoco> lmsAddressTypePocos { get; set; }
        DbSet<Lms_BankPoco> lmsBankPocos { get; set; }
        DbSet<Lms_BillPaymentPoco> lmsBillPaymentPocos { get; set; }
        DbSet<Lms_ChartOfAccountPoco> lmsChartOfAccountPocos { get; set; }
        DbSet<Lms_CompanyBranchInfoPoco> lmsCompanyBranchInfoPocos { get; set; }
        DbSet<Lms_CompanyInfoPoco> lmsCompanyInfoPocos { get; set; }
        DbSet<Lms_ConfigurationPoco> lmsConfigurationPocos { get; set; }
        DbSet<Lms_CustomerPoco> lmsCustomerPocos { get; set; }
        DbSet<Lms_CustomerEmployeeMappingPoco> lmsCustomerEmployeeMappingPocos { get; set; }
        DbSet<Lms_DeliveryOptionPoco> lmsDeliveryOptionPocos { get; set; }
        DbSet<Lms_EmployeePoco> lmsEmployeePocos { get; set; }
        DbSet<Lms_EmployeeLoanPoco> lmsEmployeeLoanPocos { get; set; }
        DbSet<Lms_EmployeeLoanPaymentPoco> lmsEmployeeLoanPaymentPocos { get; set; }
        DbSet<Lms_EmployeePaymentPoco> lmsEmployeePaymentPocos { get; set; }
        DbSet<Lms_EmployeePayrollPoco> lmsEmployeePayrollPocos { get; set; }
        DbSet<Lms_EmployeePayrollDetailPoco> lmsEmployeePayrollDetailPocos { get; set; }
        DbSet<Lms_EmployeeTimesheetPoco> lmsEmployeeTimesheetPocos { get; set; }
        DbSet<Lms_EmployeeTypePoco> lmsEmployeeTypePocos { get; set; }
        DbSet<Lms_EmployeeVehicleMappingPoco> lmsEmployeeVehicleMappingPocos { get; set; }
        DbSet<Lms_InvoicePoco> lmsInvoicePocos { get; set; }
        DbSet<Lms_InvoiceStorageOrderMappingPoco> lmsInvoiceStorageOrderMappingPocos { get; set; }
        DbSet<Lms_InvoiceWayBillMappingPoco> lmsInvoiceWayBillMappingPocos { get; set; }
        DbSet<Lms_OrderPoco> lmsOrderPocos { get; set; }
        DbSet<Lms_OrderDocumentPoco> lmsOrderDocumentPocos { get; set; }
        DbSet<Lms_OrderStatusPoco> lmsOrderStatusPocos { get; set; }
        DbSet<Lms_OrderTypePoco> lmsOrderTypePocos { get; set; }
        DbSet<Lms_OrderAdditionalServicePoco> _orderAdditionalServicePocos { get; set; }
        DbSet<Lms_PayeePoco> lmsPayeePocos { get; set; }
        DbSet<Lms_PaymentMethodPoco> lmsPaymentMethodPocos { get; set; }
        DbSet<Lms_StorageOrderPoco> lmsStorageOrderPoco { get; set; }
        DbSet<Lms_StorageOrderAdditionalServicePoco> lmsStorageOrderAdditionalServicePocos { get; set; }
        DbSet<Lms_StorageOrderDeliveryPoco> lmsStorageOrderDeliveryPocos { get; set; }
        DbSet<Lms_TariffPoco> lmsTariffPocos { get; set; }
        DbSet<Lms_TransactionPoco> lmsTransactionPocos { get; set; }
        DbSet<Lms_UnitTypePoco> lmsUnitTypePocos { get; set; }

        DbSet<Lms_VehicleTypePoco> lmsVehicleTypePocos { get; set; }
        DbSet<Lms_VehicleUnitMappingPoco> lmsVehicleUnitMappingPocos { get; set; }
        DbSet<Lms_WeightScalePoco> lmsWeightScalePocos { get; set; }

        DbSet<Lms_BillPayment_AuditPoco> lmsBillPayment_AuditPocos { get; set; }
        DbSet<Lms_Customer_AuditPoco> lmsCustomer_AuditPocos { get; set; }
        DbSet<Lms_Employee_AuditPoco> lmsEmployee_AuditPocos { get; set; }
        DbSet<Lms_EmployeeLoan_AuditPoco> lmsEmployeeLoan_AuditPocos { get; set; }
        DbSet<Lms_EmployeeLoanPayment_AuditPoco> lmsEmployeeLoanPayment_AuditPocos { get; set; }
        DbSet<Lms_EmployeePayment_AuditPoco> lmsEmployeePayment_AuditPocos { get; set; }
        DbSet<Lms_EmployeeTimesheet_AuditPoco> lmsEmployeeTimesheet_AuditPocos { get; set; }
        DbSet<Lms_Invoice_AuditPoco> lmsInvoice_AuditPocos { get; set; }
        DbSet<Lms_InvoiceStorageOrderMapping_AuditPoco> lmsInvoiceStorageOrderMapping_AuditPocos { get; set; }
        DbSet<Lms_InvoiceWayBillMapping_AuditPoco> lmsInvoiceWayBillMapping_AuditPocos { get; set; }
        DbSet<Lms_Order_AuditPoco> lmsOrder_AuditPocos { get; set; }
        DbSet<Lms_OrderStatus_AuditPoco> lmsOrderStatus_AuditPocos { get; set; }
        DbSet<Lms_StorageOrder_AuditPoco> lmsStorageOrder_AuditPocos { get; set; }
        DbSet<Lms_StorageOrderDelivery_AuditPoco> lmsStorageOrderDelivery_AuditPocos { get; set; }

        DbQuery<Lms_StoredProcedureResult> returnedValueString { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lms_AccTransactionDetailPoco>().HasKey(table => new { table.Id, table.TransactionNo });
            modelBuilder.Entity<Lms_StorageOrderAdditionalServicePoco>().HasKey(table => new { table.Id, table.AdditionalServiceId });

        }

    }
}
