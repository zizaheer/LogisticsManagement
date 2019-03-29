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
    public class CompanyInfoController : Controller
    {
        private Lms_CompanyInfoLogic _companyInfoLogic;
        private readonly LogisticsContext _dbContext;

        public CompanyInfoController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _companyInfoLogic = new Lms_CompanyInfoLogic(new EntityFrameworkGenericRepository<Lms_CompanyInfoPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _companyInfoLogic.GetList();
            return View();
        }
    }
}