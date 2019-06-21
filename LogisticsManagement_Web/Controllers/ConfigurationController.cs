using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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

        public ConfigurationController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
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