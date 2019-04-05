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
    public class CustomerEmployeeMappingController : Controller
    {
        private Lms_CustomerEmployeeMappingLogic _customerEmployeeMappingLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _memoryCache;

        public CustomerEmployeeMappingController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _customerEmployeeMappingLogic = new Lms_CustomerEmployeeMappingLogic(new EntityFrameworkGenericRepository<Lms_CustomerEmployeeMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _customerEmployeeMappingLogic.GetList();
            return View();
        }
    }
}