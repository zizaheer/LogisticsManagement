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
        IMemoryCache _cache;

        public CountryController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var countryList = _countryLogic.GetList();
            return View();
        }

        [HttpGet]
        public JsonResult GetAllCountries()
        {
            string result = "";
            try
            {
                var countryList = _countryLogic.GetList();
                if (countryList != null && countryList.Count > 0)
                {
                    result = JsonConvert.SerializeObject(countryList);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

    }
}