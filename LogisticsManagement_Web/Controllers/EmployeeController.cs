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
    public class EmployeeController : Controller
    {
        private Lms_EmployeeLogic _employeeLogic;
        private readonly LogisticsContext _dbContext;

        public EmployeeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeeLogic = new Lms_EmployeeLogic(new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
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
    }
}