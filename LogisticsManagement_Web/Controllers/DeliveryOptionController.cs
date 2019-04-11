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
    public class DeliveryOptionController : Controller
    {
        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public DeliveryOptionController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(_cache, new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _deliveryOptionLogic.GetList();
            return View();
        }
    }
}