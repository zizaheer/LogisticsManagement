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
    public class ChartOfAccountController : Controller
    {
        private Lms_ChartOfAccountLogic _chartOfAccountLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _memoryCache;

        public ChartOfAccountController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _chartOfAccountLogic = new Lms_ChartOfAccountLogic(new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _chartOfAccountLogic.GetList();
            return View();
        }
    }
}