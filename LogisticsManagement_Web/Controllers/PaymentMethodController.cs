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
    public class PaymentMethodController : Controller
    {
        private Lms_PaymentMethodLogic _paymentMethodLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public PaymentMethodController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _paymentMethodLogic = new Lms_PaymentMethodLogic(_cache, new EntityFrameworkGenericRepository<Lms_PaymentMethodPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _paymentMethodLogic.GetList();
            return View();
        }
    }
}