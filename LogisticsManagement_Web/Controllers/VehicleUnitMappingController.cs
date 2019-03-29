using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using Microsoft.AspNetCore.Mvc;


namespace LogisticsManagement_Web.Controllers
{
    public class VehicleUnitMappingController : Controller
    {
        private Lms_VehicleUnitMappingLogic _vehicleUnitMappingLogic;
        private readonly LogisticsContext _dbContext;

        public VehicleUnitMappingController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _vehicleUnitMappingLogic = new Lms_VehicleUnitMappingLogic(new EntityFrameworkGenericRepository<Lms_VehicleUnitMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _vehicleUnitMappingLogic.GetList();
            return View();
        }
    }
}