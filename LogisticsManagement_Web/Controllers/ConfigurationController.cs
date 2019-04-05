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
        private readonly LogisticsContext _dbContext;
        IMemoryCache _memoryCache;

        public ConfigurationController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _configurationLogic = new Lms_ConfigurationLogic(new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ViewBag.TaxToCall = Enum.GetValues(typeof(TaxToCall)).Cast<TaxToCall>();
            var configuration = _configurationLogic.GetSingleById(1);
            return View(configuration);
        }

        public IActionResult Update(int id)
        {
            return View();
        }
    }
}