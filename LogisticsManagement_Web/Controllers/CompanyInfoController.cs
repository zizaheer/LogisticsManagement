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
    public class CompanyInfoController : Controller
    {
        private Lms_CompanyInfoLogic _companyInfoLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public CompanyInfoController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _companyInfoLogic = new Lms_CompanyInfoLogic(_cache, new EntityFrameworkGenericRepository<Lms_CompanyInfoPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _companyInfoLogic.GetList();
            return View();
        }
    }
}