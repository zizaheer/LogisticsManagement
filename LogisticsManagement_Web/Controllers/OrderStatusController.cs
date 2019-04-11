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
    public class OrderStatusController : Controller
    {
        private Lms_OrderStatusLogic _orderStatusLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public OrderStatusController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _orderStatusLogic.GetList();
            return View();
        }
    }
}