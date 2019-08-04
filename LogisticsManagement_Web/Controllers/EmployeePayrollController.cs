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
    public class EmployeePayrollController : Controller
    {
        private Lms_EmployeePayrollLogic _employeePayrollLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public EmployeePayrollController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _employeePayrollLogic = new Lms_EmployeePayrollLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePayrollPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();
            ViewBag.EmployeeTypes = Enum.GetValues(typeof(Enum_EmployeeType)).Cast<Enum_EmployeeType>();

            var employeeController = new EmployeeController(_cache,_dbContext);
            employeeController.GetEmployeeData();
            return View(employeeController.GetEmployeeData());
        }

        private void ValidateSession()
        {
            if (HttpContext.Session.GetString("SessionData") != null)
            {
                sessionData = JsonConvert.DeserializeObject<SessionData>(HttpContext.Session.GetString("SessionData"));
                if (sessionData == null)
                {
                    Response.Redirect("Login/Index");
                }
            }
            else
            {
                Response.Redirect("Login/InvalidLocation");
            }
        }
    }
}