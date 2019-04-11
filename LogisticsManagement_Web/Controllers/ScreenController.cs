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
    public class ScreenController : Controller
    {
        private App_ScreenLogic _screenLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public ScreenController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _screenLogic = new App_ScreenLogic(_cache, new EntityFrameworkGenericRepository<App_ScreenPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _screenLogic.GetList();
            return View();
        }
    }
}