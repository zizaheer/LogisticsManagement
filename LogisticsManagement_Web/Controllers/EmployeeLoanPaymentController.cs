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
    public class EmployeeLoanPaymentController : Controller
    {
        private Lms_EmployeeLoanPaymentLogic _employeeLoanPaymentLogic;
        private readonly LogisticsContext _dbContext;

        public EmployeeLoanPaymentController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeeLoanPaymentLogic = new Lms_EmployeeLoanPaymentLogic(new EntityFrameworkGenericRepository<Lms_EmployeeLoanPaymentPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _employeeLoanPaymentLogic.GetAllList();
            return View();
        }
    }
}