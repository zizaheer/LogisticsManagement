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
    public class CompanyBranchInfoController : Controller
    {
        private Lms_CompanyBranchInfoLogic _companyBranchInfoLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _memoryCache;

        public CompanyBranchInfoController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _companyBranchInfoLogic = new Lms_CompanyBranchInfoLogic(new EntityFrameworkGenericRepository<Lms_CompanyBranchInfoPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _companyBranchInfoLogic.GetList();
            return View();
        }
    }
}