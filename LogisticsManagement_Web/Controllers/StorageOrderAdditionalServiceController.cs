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
    public class StorageOrderAdditionalServiceController : Controller
    {
        private Lms_StorageOrderAdditionalServiceLogic _storageOrderAdditionalServiceLogic;
        private readonly LogisticsContext _dbContext;

        public StorageOrderAdditionalServiceController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _storageOrderAdditionalServiceLogic = new Lms_StorageOrderAdditionalServiceLogic(new EntityFrameworkGenericRepository<Lms_StorageOrderAdditionalServicePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _storageOrderAdditionalServiceLogic.GetList();
            return View();
        }
    }
}