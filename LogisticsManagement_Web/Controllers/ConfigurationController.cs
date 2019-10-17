using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogisticsManagement_Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private Lms_ConfigurationLogic _configurationLogic;
        private Lms_UnitTypeLogic _unitTypeLogic;
        private Lms_WeightScaleLogic _weightScaleLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration, IMemoryCache cache, IHostingEnvironment hostingEnvironment, LogisticsContext dbContext)
        {
            _cache = cache;
            _configuration = configuration;
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));

        }

        public IActionResult Index()
        {
            ValidateSession();

            ViewBag.TaxToCall = Enum.GetValues(typeof(Enum_TaxToCall)).Cast<Enum_TaxToCall>();
            var configuration = _configurationLogic.GetSingleById(1);
            _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
            _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));

            ViewBag.UnitTypes = _unitTypeLogic.GetList();
            ViewBag.WeightScales = _weightScaleLogic.GetList();
            return View(configuration);
        }

        [HttpPost]
        public IActionResult Update([FromBody]dynamic configurationData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (configurationData != null)
                {
                    Lms_ConfigurationPoco configData = JsonConvert.DeserializeObject<Lms_ConfigurationPoco>(JsonConvert.SerializeObject(configurationData));

                    var existingConfigData = _configurationLogic.GetSingleById(configData.Id);

                    if (existingConfigData.Id > 0)
                    {
                        existingConfigData.TaxAmount = configData.TaxAmount;
                        existingConfigData.TaxToCall = configData.TaxToCall;
                        existingConfigData.IsSignInRequiredForDispatch = configData.IsSignInRequiredForDispatch;
                        existingConfigData.WayBillPrefix = configData.WayBillPrefix;
                        existingConfigData.DeliveryWBNoStartFrom = configData.DeliveryWBNoStartFrom;
                        existingConfigData.MiscWBNoStartFrom = configData.MiscWBNoStartFrom;
                        existingConfigData.StorageWBNoStartFrom = configData.StorageWBNoStartFrom;
                        existingConfigData.InvoiceNumberStartFrom = configData.InvoiceNumberStartFrom;
                        existingConfigData.DefaultFuelSurcharge = configData.DefaultFuelSurcharge;

                        existingConfigData.ParentGLForCustomerAccount = configData.ParentGLForCustomerAccount;
                        existingConfigData.ParentGLForEmployeeAccount = configData.ParentGLForEmployeeAccount;
                        existingConfigData.SalesTaxPayableAccount = configData.SalesTaxPayableAccount;
                        existingConfigData.SalesIncomeAccount = configData.SalesIncomeAccount;
                        existingConfigData.SalaryExpenseAccount = configData.SalaryExpenseAccount;
                        existingConfigData.BonusExpenseAccount = configData.BonusExpenseAccount;
                        existingConfigData.OtherReceivableAccount = configData.OtherReceivableAccount;
                        existingConfigData.OtherPayableAccount = configData.OtherPayableAccount;
                        existingConfigData.OtherIncomeAccount = configData.OtherIncomeAccount;
                        existingConfigData.OtherExpenseAccount = configData.OtherExpenseAccount;
                        existingConfigData.BankAccount = configData.BankAccount;
                        existingConfigData.CashAccount = configData.CashAccount;

                        // existingConfigData.DefaultWeightScaleId = null;

                        _configurationLogic.Update(existingConfigData);

                        result = "Success";
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult ClearCache()
        {
            ValidateSession();
            var result = "";

            try
            {
                _cache.Remove(App_CacheKeys.Cities);
                _cache.Remove(App_CacheKeys.Countries);
                _cache.Remove(App_CacheKeys.Provinces);
                _cache.Remove(App_CacheKeys.Tariffs);
                _cache.Remove(App_CacheKeys.Addresses);
                _cache.Remove(App_CacheKeys.Customers);
                _cache.Remove(App_CacheKeys.CustomerAddresses);
                _cache.Remove(App_CacheKeys.Employees);
                _cache.Remove(App_CacheKeys.Orders);
                _cache.Remove(App_CacheKeys.OrderAdditionalServices);
                _cache.Remove(App_CacheKeys.OrderStatuses);
                _cache.Remove(App_CacheKeys.AccountBalances);
                _cache.Remove(App_CacheKeys.AdditionalServices);
                _cache.Remove(App_CacheKeys.Accounts);
                _cache.Remove(App_CacheKeys.EmployeePayrolls);
                _cache.Remove(App_CacheKeys.EmployeePayrollDetails);
                _cache.Remove(App_CacheKeys.Invoices);
                _cache.Remove(App_CacheKeys.InvoiceWayBillMappings);
                _cache.Remove(App_CacheKeys.City);
                _cache.Remove(App_CacheKeys.Country);
                _cache.Remove(App_CacheKeys.Province);
                _cache.Remove(App_CacheKeys.Tariff);
                _cache.Remove(App_CacheKeys.Address);
                _cache.Remove(App_CacheKeys.Customer);
                _cache.Remove(App_CacheKeys.Employee);

                result = "Success";
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult DeleteInvoiceWayBill()
        {
            ValidateSession();
            var result = "";

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(_hostingEnvironment.WebRootPath + "/contents/invoices/");
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                directoryInfo = new DirectoryInfo(_hostingEnvironment.WebRootPath + "/contents/waybills/");
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                result = "Success";
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        public IActionResult CreateDatabaseBackup()
        {

            string backupLocation = _hostingEnvironment.WebRootPath + "/contents/database/";

            if (!Directory.Exists(backupLocation))
            {
                Directory.CreateDirectory(backupLocation);
            }

            var fileName = "DbBackup_" + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss") + ".bak";

            _configurationLogic.CreateDatabaseBackup(backupLocation, fileName);

            return null;
        }

        private void ValidateSession()
        {
            if (HttpContext.Session.GetString("SessionData") != null)
            {
                sessionData = JsonConvert.DeserializeObject<SessionData>(HttpContext.Session.GetString("SessionData"));
                if (sessionData == null)
                {
                    Response.Redirect("Login/Index");
                }
            }
            else
            {
                Response.Redirect("Login/InvalidLocation");
            }
        }


    }
}