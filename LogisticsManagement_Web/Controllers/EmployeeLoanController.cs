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
    public class EmployeeLoanController : Controller
    {
        private Lms_EmployeeLoanLogic employeeLoanLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public EmployeeLoanController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            employeeLoanLogic = new Lms_EmployeeLoanLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeLoanPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = employeeLoanLogic.GetList();
            return View();
        }
    }
}