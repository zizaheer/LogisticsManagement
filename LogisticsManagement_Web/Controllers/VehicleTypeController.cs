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
    public class VehicleTypeController : Controller
    {
        private Lms_VehicleTypeLogic _vehicleTypeLogic;
        private readonly LogisticsContext _dbContext;

        public VehicleTypeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _vehicleTypeLogic = new Lms_VehicleTypeLogic(new EntityFrameworkGenericRepository<Lms_VehicleTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _vehicleTypeLogic.GetList();
            return View();
        }
    }
}