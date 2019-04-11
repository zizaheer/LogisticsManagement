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
    public class EmployeeLoanPaymentController : Controller
    {
        private Lms_EmployeeLoanPaymentLogic _employeeLoanPaymentLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public EmployeeLoanPaymentController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeeLoanPaymentLogic = new Lms_EmployeeLoanPaymentLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeLoanPaymentPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _employeeLoanPaymentLogic.GetList();
            return View();
        }
    }
}