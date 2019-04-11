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
    public class EmployeeTypeController : Controller
    {
        private Lms_EmployeeTypeLogic _employeeTypeLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public EmployeeTypeController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeeTypeLogic = new Lms_EmployeeTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _employeeTypeLogic.GetList();
            return View();
        }
    }
}