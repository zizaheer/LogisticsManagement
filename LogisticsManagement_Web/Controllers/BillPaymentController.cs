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
    public class BillPaymentController : Controller
    {
        private Lms_BillPaymentLogic _billPaymentLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public BillPaymentController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _billPaymentLogic = new Lms_BillPaymentLogic(_cache, new EntityFrameworkGenericRepository<Lms_BillPaymentPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _billPaymentLogic.GetList();
            return View();
        }
    }
}