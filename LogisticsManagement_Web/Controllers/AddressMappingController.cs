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
    public class AddressMappingController : Controller
    {
        private Lms_AddressMappingLogic _addressMappingLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _memoryCache;

        public AddressMappingController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _addressMappingLogic = new Lms_AddressMappingLogic(new EntityFrameworkGenericRepository<Lms_AddressMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _addressMappingLogic.GetList();
            return View();
        }
    }
}