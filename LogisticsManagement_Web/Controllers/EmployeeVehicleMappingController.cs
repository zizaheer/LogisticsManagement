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
    public class EmployeeVehicleMappingController : Controller
    {
        private Lms_EmployeeVehicleMappingLogic _employeeVehicleMappingLogic;
        private readonly LogisticsContext _dbContext;

        public EmployeeVehicleMappingController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeeVehicleMappingLogic = new Lms_EmployeeVehicleMappingLogic(new EntityFrameworkGenericRepository<Lms_EmployeeVehicleMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _employeeVehicleMappingLogic.GetList();
            return View();
        }
    }
}