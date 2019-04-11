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
    public class WeightScaleController : Controller
    {
        private Lms_WeightScaleLogic _weightScaleLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public WeightScaleController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _weightScaleLogic.GetList();
            return View();
        }
    }
}