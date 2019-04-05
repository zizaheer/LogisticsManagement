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
    public class CityController : Controller
    {
        private App_CityLogic _cityLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        public CityController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _cityLogic = new App_CityLogic(cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _cityLogic.GetList();
            return View();
        }
    }
}