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
    public class StorageOrderController : Controller
    {
        private Lms_StorageOrderLogic _storageOrderLogic;
        private readonly LogisticsContext _dbContext;

        public StorageOrderController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _storageOrderLogic = new Lms_StorageOrderLogic(new EntityFrameworkGenericRepository<Lms_StorageOrderPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _storageOrderLogic.GetList();
            return View();
        }
    }
}