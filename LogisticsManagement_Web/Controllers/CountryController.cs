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
    public class CountryController : Controller
    {
        private App_CountryLogic _countryLogic;
        private readonly LogisticsContext _dbContext;

        public CountryController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _countryLogic = new App_CountryLogic(new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _countryLogic.GetAllList();
            return View();
        }
    }
}