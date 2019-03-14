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
    public class EmployeeTypeController : Controller
    {
        private Lms_EmployeeTypeLogic _employeeTypeLogic;
        private readonly LogisticsContext _dbContext;

        public EmployeeTypeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeeTypeLogic = new Lms_EmployeeTypeLogic(new EntityFrameworkGenericRepository<Lms_EmployeeTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _employeeTypeLogic.GetAllList();
            return View();
        }
    }
}