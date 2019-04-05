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
    public class EmployeeTimesheetController : Controller
    {
        private Lms_EmployeeTimesheetLogic _employeeTimesheetLogic;
        private readonly LogisticsContext _dbContext;

        public EmployeeTimesheetController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeeTimesheetLogic = new Lms_EmployeeTimesheetLogic(new EntityFrameworkGenericRepository<Lms_EmployeeTimesheetPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _employeeTimesheetLogic.GetList();
            return View();
        }
    }
}