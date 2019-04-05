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
    public class CustomerController : Controller
    {
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_EmployeeLogic _employeeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private readonly LogisticsContext _dbContext;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public CustomerController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;

            _dbContext = dbContext;
            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();
            return View(GetCustomerData());
        }


        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewCustomerData", GetCustomerData());
        }


        [HttpPost]
        public IActionResult AddOrUpdate([FromBody]dynamic customerData)
        {
            ValidateSession();
            var result = false;
            try
            {
                if (customerData != null)
                {
                    Lms_CustomerPoco customerPoco = JsonConvert.DeserializeObject<Lms_CustomerPoco>(JsonConvert.SerializeObject(customerData[0]));
                    Lms_AddressPoco billingAddressPoco = JsonConvert.DeserializeObject<Lms_AddressPoco>(JsonConvert.SerializeObject(customerData[1]));
                    Lms_AddressPoco mailingAddressPoco = JsonConvert.DeserializeObject<Lms_AddressPoco>(JsonConvert.SerializeObject(customerData[2]));

                    var addressData = GetCustomerData().Addressess;

                    if (customerPoco.Id > 0)
                    {
                        _customerLogic.Update(customerPoco);
                    }
                    else
                    {
                        Lms_ChartOfAccountLogic _chartOfAccountLogic = new Lms_ChartOfAccountLogic(new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));
                        var _chartOfAccountPoco = new Lms_ChartOfAccountPoco();

                        _chartOfAccountPoco.AccountTypeId = 1;
                        _chartOfAccountPoco.AccountName = customerPoco.CustomerName;
                        _chartOfAccountPoco.BranchId = (int)sessionData.BranchId;
                        _chartOfAccountPoco.InitialBalance = 0;
                        _chartOfAccountPoco.IsActive = true;
                        _chartOfAccountPoco.Remarks = "Customer Account";
                        _chartOfAccountPoco.CreatedBy = sessionData.UserId;

                        customerPoco.CustomerNumber = (_customerLogic.GetMaxId() + 1).ToString();
                        customerPoco.AccountId = _chartOfAccountLogic.Add(_chartOfAccountPoco).Id;

                        if (billingAddressPoco.Id < 1)
                        {
                            var matchingAddress1 = addressData.Where(c => c.UnitNumber == billingAddressPoco.UnitNumber && c.AddressLine == billingAddressPoco.AddressLine).FirstOrDefault();
                            var matchingAddress2 = addressData.Where(c => c.UnitNumber == mailingAddressPoco.UnitNumber && c.AddressLine == mailingAddressPoco.AddressLine).FirstOrDefault();
                            if (matchingAddress1 == null)
                            {
                                Lms_AddressLogic _addressLogic = new Lms_AddressLogic(new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                                _addressLogic.Add(billingAddressPoco);
                            }

                        }






                        _customerLogic.Add(customerPoco);
                    }

                    result = true;
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Remove([FromBody]dynamic customerData)
        {
            bool result = false;
            try
            {
                var serializedData = JsonConvert.SerializeObject(customerData);
                Lms_CustomerPoco[] pocos = JsonConvert.DeserializeObject<Lms_CustomerPoco[]>(serializedData);

                _customerLogic.Remove(pocos);
                result = true;
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        private CustomerViewModel GetCustomerData()
        {
            CustomerViewModel customerViewModel = new CustomerViewModel();
            List<Lms_AddressPoco> _addresses;
            List<Lms_CustomerPoco> _customers;
            List<Lms_EmployeePoco> _employees;
            List<App_CityPoco> _cities;
            List<App_ProvincePoco> _provinces;
            List<App_CountryPoco> _countries;

            #region Set Caching 
            // TO-DO: This set-get caching mechanism should be optimized in a seperate function

            //if (!_cache.TryGetValue(CacheKeys.Addresses, out _addresses))
            //{
            //    _addressLogic = new Lms_AddressLogic(new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            //    _addresses = _addressLogic.GetList();

            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromDays(3));
            //    _cache.Set(CacheKeys.Addresses, _addresses, cacheEntryOptions);
            //}

            //if (!_cache.TryGetValue(CacheKeys.Customers, out _customers))
            //{
            //    _customers = _customerLogic.GetList();
            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromDays(3));
            //    _cache.Set(CacheKeys.Customers, _customers, cacheEntryOptions);
            //}

            //if (!_cache.TryGetValue(CacheKeys.Employees, out _employees))
            //{
            //    _employeeLogic = new Lms_EmployeeLogic(new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            //    _employees = _employeeLogic.GetList();

            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromDays(3));
            //    _cache.Set(CacheKeys.Employees, _employees, cacheEntryOptions);
            //}

            //if (!_cache.TryGetValue(CacheKeys.Cities, out _cities))
            //{
            //    _cityLogic = new App_CityLogic(new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            //    _cities = _cityLogic.GetList();

            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromDays(3));
            //    _cache.Set(CacheKeys.Cities, _cities, cacheEntryOptions);
            //}

            //if (!_cache.TryGetValue(CacheKeys.Provinces, out _provinces))
            //{
            //    _provinceLogic = new App_ProvinceLogic(new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            //    _provinces = _provinceLogic.GetList();

            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromDays(3));
            //    _cache.Set(CacheKeys.Provinces, _provinces, cacheEntryOptions);
            //}

            //if (!_cache.TryGetValue(CacheKeys.Countries, out _countries))
            //{
            //    _countryLogic = new App_CountryLogic(new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
            //    _countries = _countryLogic.GetList();

            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetSlidingExpiration(TimeSpan.FromDays(3));
            //    _cache.Set(CacheKeys.Countries, _countries, cacheEntryOptions);
            //}

            #endregion


            customerViewModel.Customers = _customerLogic.GetList();

            //customerViewModel.Addressess = _cache.Get<List<Lms_AddressPoco>>(CacheKeys.Addresses);
            //customerViewModel.Customers = _cache.Get<List<Lms_CustomerPoco>>(CacheKeys.Customers);
            //customerViewModel.Employees = _cache.Get<List<Lms_EmployeePoco>>(CacheKeys.Employees);
            //customerViewModel.Cities = _cache.Get<List<App_CityPoco>>(CacheKeys.Cities);
            //customerViewModel.Provinces = _cache.Get<List<App_ProvincePoco>>(CacheKeys.Provinces);
            //customerViewModel.Countries = _cache.Get<List<App_CountryPoco>>(CacheKeys.Countries);

            return customerViewModel;
        }


        public JsonResult GetCustomers()
        {
            return Json(JsonConvert.SerializeObject(_customerLogic.GetList()));
        }

        private void ValidateSession()
        {
            sessionData = JsonConvert.DeserializeObject<SessionData>(HttpContext.Session.GetString("SessionData"));
            if (sessionData == null)
            {
                Response.Redirect("Login/Index");
            }
        }
    }
}