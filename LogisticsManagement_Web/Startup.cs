using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_Poco;
using Rotativa.AspNetCore;
using LogisticsManagement_Web.Services;

namespace LogisticsManagement_Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHttpContextAccessor();

            services.AddTransient<IEmailService, EmailService>();
            services.AddDistributedMemoryCache();
            services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(20));

            services.AddMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton(Configuration);

            services.AddScoped<IDataRepository<App_CityPoco>, EntityFrameworkGenericRepository<App_CityPoco>>();
            services.AddScoped<IDataRepository<App_CountryPoco>, EntityFrameworkGenericRepository<App_CountryPoco>>();
            services.AddScoped<IDataRepository<App_DocumentCategoryPoco>, EntityFrameworkGenericRepository<App_DocumentCategoryPoco>>();
            services.AddScoped<IDataRepository<App_DocumentTypePoco>, EntityFrameworkGenericRepository<App_DocumentTypePoco>>();
            services.AddScoped<IDataRepository<App_LoginHistoryPoco>, EntityFrameworkGenericRepository<App_LoginHistoryPoco>>();
            services.AddScoped<IDataRepository<App_ProvincePoco>, EntityFrameworkGenericRepository<App_ProvincePoco>>();
            services.AddScoped<IDataRepository<App_ScreenPoco>, EntityFrameworkGenericRepository<App_ScreenPoco>>();
            services.AddScoped<IDataRepository<App_ScreenPermissionPoco>, EntityFrameworkGenericRepository<App_ScreenPermissionPoco>>();
            services.AddScoped<IDataRepository<App_UserPoco>, EntityFrameworkGenericRepository<App_UserPoco>>();
            services.AddScoped<IDataRepository<App_UserGroupPoco>, EntityFrameworkGenericRepository<App_UserGroupPoco>>();
            services.AddScoped<IDataRepository<Lms_ParentGLCodePoco>, EntityFrameworkGenericRepository<Lms_ParentGLCodePoco>>();
            services.AddScoped<IDataRepository<Lms_AccountTypePoco>, EntityFrameworkGenericRepository<Lms_AccountTypePoco>>();
            services.AddScoped<IDataRepository<Lms_AdditionalServicePoco>, EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>>();
            services.AddScoped<IDataRepository<Lms_AddressPoco>, EntityFrameworkGenericRepository<Lms_AddressPoco>>();
            services.AddScoped<IDataRepository<Lms_AddressMappingPoco>, EntityFrameworkGenericRepository<Lms_AddressMappingPoco>>();
            services.AddScoped<IDataRepository<Lms_AddressTypePoco>, EntityFrameworkGenericRepository<Lms_AddressTypePoco>>();
            services.AddScoped<IDataRepository<Lms_BankPoco>, EntityFrameworkGenericRepository<Lms_BankPoco>>();
            services.AddScoped<IDataRepository<Lms_BillPaymentPoco>, EntityFrameworkGenericRepository<Lms_BillPaymentPoco>>();
            services.AddScoped<IDataRepository<Lms_ChartOfAccountPoco>, EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>>();
            services.AddScoped<IDataRepository<Lms_CompanyBranchInfoPoco>, EntityFrameworkGenericRepository<Lms_CompanyBranchInfoPoco>>();
            services.AddScoped<IDataRepository<Lms_CompanyInfoPoco>, EntityFrameworkGenericRepository<Lms_CompanyInfoPoco>>();
            services.AddScoped<IDataRepository<Lms_ConfigurationPoco>, EntityFrameworkGenericRepository<Lms_ConfigurationPoco>>();
            services.AddScoped<IDataRepository<Lms_CustomerPoco>, EntityFrameworkGenericRepository<Lms_CustomerPoco>>();
            services.AddScoped<IDataRepository<Lms_CustomerEmployeeMappingPoco>, EntityFrameworkGenericRepository<Lms_CustomerEmployeeMappingPoco>>();
            services.AddScoped<IDataRepository<Lms_DeliveryOptionPoco>, EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeePoco>, EntityFrameworkGenericRepository<Lms_EmployeePoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeeLoanPoco>, EntityFrameworkGenericRepository<Lms_EmployeeLoanPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeeLoanPaymentPoco>, EntityFrameworkGenericRepository<Lms_EmployeeLoanPaymentPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeePaymentPoco>, EntityFrameworkGenericRepository<Lms_EmployeePaymentPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeePayrollPoco>, EntityFrameworkGenericRepository<Lms_EmployeePayrollPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeePayrollDetailPoco>, EntityFrameworkGenericRepository<Lms_EmployeePayrollDetailPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeeTimesheetPoco>, EntityFrameworkGenericRepository<Lms_EmployeeTimesheetPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeeTypePoco>, EntityFrameworkGenericRepository<Lms_EmployeeTypePoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeeVehicleMappingPoco>, EntityFrameworkGenericRepository<Lms_EmployeeVehicleMappingPoco>>();
            services.AddScoped<IDataRepository<Lms_InvoicePoco>, EntityFrameworkGenericRepository<Lms_InvoicePoco>>();
            services.AddScoped<IDataRepository<Lms_InvoiceStorageOrderMappingPoco>, EntityFrameworkGenericRepository<Lms_InvoiceStorageOrderMappingPoco>>();
            services.AddScoped<IDataRepository<Lms_InvoiceWayBillMappingPoco>, EntityFrameworkGenericRepository<Lms_InvoiceWayBillMappingPoco>>();
            services.AddScoped<IDataRepository<Lms_OrderPoco>, EntityFrameworkGenericRepository<Lms_OrderPoco>>();
            services.AddScoped<IDataRepository<Lms_OrderDocumentPoco>, EntityFrameworkGenericRepository<Lms_OrderDocumentPoco>>();
            services.AddScoped<IDataRepository<Lms_OrderStatusPoco>, EntityFrameworkGenericRepository<Lms_OrderStatusPoco>>();
            services.AddScoped<IDataRepository<Lms_OrderTypePoco>, EntityFrameworkGenericRepository<Lms_OrderTypePoco>>();
            services.AddScoped<IDataRepository<Lms_OrderAdditionalServicePoco>, EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>>();
            services.AddScoped<IDataRepository<Lms_PayeePoco>, EntityFrameworkGenericRepository<Lms_PayeePoco>>();
            services.AddScoped<IDataRepository<Lms_PaymentMethodPoco>, EntityFrameworkGenericRepository<Lms_PaymentMethodPoco>>();
            services.AddScoped<IDataRepository<Lms_StorageOrderPoco>, EntityFrameworkGenericRepository<Lms_StorageOrderPoco>>();
            services.AddScoped<IDataRepository<Lms_StorageOrderAdditionalServicePoco>, EntityFrameworkGenericRepository<Lms_StorageOrderAdditionalServicePoco>>();
            services.AddScoped<IDataRepository<Lms_StorageOrderDeliveryPoco>, EntityFrameworkGenericRepository<Lms_StorageOrderDeliveryPoco>>();
            services.AddScoped<IDataRepository<Lms_TariffPoco>, EntityFrameworkGenericRepository<Lms_TariffPoco>>();
            services.AddScoped<IDataRepository<Lms_TransactionPoco>, EntityFrameworkGenericRepository<Lms_TransactionPoco>>();
            services.AddScoped<IDataRepository<Lms_TransactionDetailPoco>, EntityFrameworkGenericRepository<Lms_TransactionDetailPoco>>();
            services.AddScoped<IDataRepository<Lms_UnitTypePoco>, EntityFrameworkGenericRepository<Lms_UnitTypePoco>>();
            services.AddScoped<IDataRepository<Lms_VehicleTypePoco>, EntityFrameworkGenericRepository<Lms_VehicleTypePoco>>();
            services.AddScoped<IDataRepository<Lms_VehicleUnitMappingPoco>, EntityFrameworkGenericRepository<Lms_VehicleUnitMappingPoco>>();
            services.AddScoped<IDataRepository<Lms_WeightScalePoco>, EntityFrameworkGenericRepository<Lms_WeightScalePoco>>();
            services.AddScoped<IDataRepository<Lms_BillPayment_AuditPoco>, EntityFrameworkGenericRepository<Lms_BillPayment_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_Customer_AuditPoco>, EntityFrameworkGenericRepository<Lms_Customer_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_Employee_AuditPoco>, EntityFrameworkGenericRepository<Lms_Employee_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeeLoan_AuditPoco>, EntityFrameworkGenericRepository<Lms_EmployeeLoan_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeeLoanPayment_AuditPoco>, EntityFrameworkGenericRepository<Lms_EmployeeLoanPayment_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeePayment_AuditPoco>, EntityFrameworkGenericRepository<Lms_EmployeePayment_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_EmployeeTimesheet_AuditPoco>, EntityFrameworkGenericRepository<Lms_EmployeeTimesheet_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_Invoice_AuditPoco>, EntityFrameworkGenericRepository<Lms_Invoice_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_InvoiceStorageOrderMapping_AuditPoco>, EntityFrameworkGenericRepository<Lms_InvoiceStorageOrderMapping_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_InvoiceWayBillMapping_AuditPoco>, EntityFrameworkGenericRepository<Lms_InvoiceWayBillMapping_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_Order_AuditPoco>, EntityFrameworkGenericRepository<Lms_Order_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_OrderStatus_AuditPoco>, EntityFrameworkGenericRepository<Lms_OrderStatus_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_StorageOrder_AuditPoco>, EntityFrameworkGenericRepository<Lms_StorageOrder_AuditPoco>>();
            services.AddScoped<IDataRepository<Lms_StorageOrderDelivery_AuditPoco>, EntityFrameworkGenericRepository<Lms_StorageOrderDelivery_AuditPoco>>();


            services.AddDbContext<LogisticsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LogisticsDbConnection")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSession(); // app.UseSession should be placed before app.UseMvc

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
            });

            RotativaConfiguration.Setup(env);

        }
    }
}
