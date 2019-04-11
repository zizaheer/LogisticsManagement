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
    public class VehicleUnitMappingController : Controller
    {
        private Lms_VehicleUnitMappingLogic _vehicleUnitMappingLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public VehicleUnitMappingController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _vehicleUnitMappingLogic = new Lms_VehicleUnitMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_VehicleUnitMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _vehicleUnitMappingLogic.GetList();
            return View();
        }
    }
}