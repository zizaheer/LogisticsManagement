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
    public class EmployeeController : Controller
    {
        private Lms_EmployeeLogic _employeeLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public EmployeeController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var employeeList = _employeeLogic.GetList();
            return View();
        }

        public IActionResult AddOrUpdate([FromBody]dynamic data)
        {

            return View();
        }


        public JsonResult GetEmployees()
        {
            var employeeList = _employeeLogic.GetList();
            return Json(JsonConvert.SerializeObject(employeeList));
        }

        public JsonResult GetEmployeeById(string id)
        {
            var employeeList = _employeeLogic.GetList();
            return Json(JsonConvert.SerializeObject(employeeList));
        }
    }
}