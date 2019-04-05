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
    public class CountryController : Controller
    {
        private App_CountryLogic _countryLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _memoryCache;

        public CountryController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _countryLogic = new App_CountryLogic(new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _countryLogic.GetList();
            return View();
        }
    }
}