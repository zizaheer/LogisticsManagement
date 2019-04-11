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
    public class BankController : Controller
    {
        private Lms_BankLogic _bankLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public BankController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _bankLogic = new Lms_BankLogic(_cache, new EntityFrameworkGenericRepository<Lms_BankPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _bankLogic.GetList();
            return View();
        }
    }
}