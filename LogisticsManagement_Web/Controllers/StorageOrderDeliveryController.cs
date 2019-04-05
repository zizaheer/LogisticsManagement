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
    public class StorageOrderDeliveryController : Controller
    {
        private Lms_StorageOrderDeliveryLogic _storageOrderDeliveryLogic;
        private readonly LogisticsContext _dbContext;

        public StorageOrderDeliveryController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _storageOrderDeliveryLogic = new Lms_StorageOrderDeliveryLogic(new EntityFrameworkGenericRepository<Lms_StorageOrderDeliveryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _storageOrderDeliveryLogic.GetList();
            return View();
        }
    }
}