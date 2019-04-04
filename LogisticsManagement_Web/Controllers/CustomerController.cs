using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
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

        IMemoryCache _memoryCache;

        public CustomerController(IMemoryCache memoryCache, LogisticsContext dbContext)
        {
            _memoryCache = memoryCache;

            _dbContext = dbContext;
            _customerLogic = new Lms_CustomerLogic(new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            return View(GetCustomerData());
        }


        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            return PartialView("_PartialViewCustomerData", GetCustomerData());
        }


        [HttpPost]
        public IActionResult AddOrUpdate([FromBody]dynamic customerData)
        {
            var result = false;
            try
            {
                var test = customerData[0];
                var serializedData = JsonConvert.SerializeObject(test);
                //var customerDataFiltered = customerData.data.ToArray();
                Lms_CustomerPoco pocos = JsonConvert.DeserializeObject<Lms_CustomerPoco>(serializedData);

                pocos.CreateDate = DateTime.Now;
                pocos.CreatedBy = 1;
                if (pocos.Id > 0)
                {
                    _customerLogic.Update(pocos);
                }
                else
                {
                    _customerLogic.Add(pocos);
                }

                result = true;
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

            if (!_memoryCache.TryGetValue(CacheKeys.Addresses, out _addresses))
            {
                _addressLogic = new Lms_AddressLogic(new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                _addresses = _addressLogic.GetList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _memoryCache.Set(CacheKeys.Addresses, _addresses, cacheEntryOptions);
            }

            if (!_memoryCache.TryGetValue(CacheKeys.Customers, out _customers))
            {
                _customers = _customerLogic.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _memoryCache.Set(CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            if (!_memoryCache.TryGetValue(CacheKeys.Employees, out _employees))
            {
                _employeeLogic = new Lms_EmployeeLogic(new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
                _employees = _employeeLogic.GetList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _memoryCache.Set(CacheKeys.Employees, _employees, cacheEntryOptions);
            }

            if (!_memoryCache.TryGetValue(CacheKeys.Cities, out _cities))
            {
                _cityLogic = new App_CityLogic(new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
                _cities = _cityLogic.GetList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _memoryCache.Set(CacheKeys.Cities, _cities, cacheEntryOptions);
            }

            if (!_memoryCache.TryGetValue(CacheKeys.Provinces, out _provinces))
            {
                _provinceLogic = new App_ProvinceLogic(new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
                _provinces = _provinceLogic.GetList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _memoryCache.Set(CacheKeys.Provinces, _provinces, cacheEntryOptions);
            }

            if (!_memoryCache.TryGetValue(CacheKeys.Countries, out _countries))
            {
                _countryLogic = new App_CountryLogic(new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
                _countries = _countryLogic.GetList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _memoryCache.Set(CacheKeys.Countries, _countries, cacheEntryOptions);
            }

            #endregion

            customerViewModel.Addressess = _memoryCache.Get<List<Lms_AddressPoco>>(CacheKeys.Addresses);
            customerViewModel.Customers = _memoryCache.Get<List<Lms_CustomerPoco>>(CacheKeys.Customers);
            customerViewModel.Employees = _memoryCache.Get<List<Lms_EmployeePoco>>(CacheKeys.Employees);
            customerViewModel.Cities = _memoryCache.Get<List<App_CityPoco>>(CacheKeys.Cities);
            customerViewModel.Provinces = _memoryCache.Get<List<App_ProvincePoco>>(CacheKeys.Provinces);
            customerViewModel.Countries = _memoryCache.Get<List<App_CountryPoco>>(CacheKeys.Countries);

            return customerViewModel;
        }


        //public JsonResult GetAll()
        //{
        //    //return Json(JsonConvert.SerializeObject(GetCustomerData()));
        //}

    }
}