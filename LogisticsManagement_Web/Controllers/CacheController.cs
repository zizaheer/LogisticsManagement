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

namespace LogisticsManagement_Web.Controllers
{
    public class CacheController
    {
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_EmployeeLogic _employeeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private readonly LogisticsContext _dbContext;

        private IMemoryCache _cache;


        public CacheController(IMemoryCache memoryCache, LogisticsContext dbContext)
        {
            _cache = memoryCache;
        }

        public List<Lms_AddressPoco> CacheAddress()
        {
            List<Lms_AddressPoco> _addresses;
            if (!_cache.TryGetValue(CacheKeys.Addresses, out _addresses))
            {

            }

            return _addresses;
        }
    }
}