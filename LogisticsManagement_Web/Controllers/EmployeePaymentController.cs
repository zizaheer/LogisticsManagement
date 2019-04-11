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
    public class EmployeePaymentController : Controller
    {
        private Lms_EmployeePaymentLogic _employeePaymentLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public EmployeePaymentController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeePaymentLogic = new Lms_EmployeePaymentLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePaymentPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _employeePaymentLogic.GetList();
            return View();
        }
    }
}