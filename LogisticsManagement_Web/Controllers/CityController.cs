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
    public class CityController : Controller
    {
        private App_CityLogic _cityLogic;
        private readonly LogisticsContext _dbContext;

        public CityController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _cityLogic = new App_CityLogic(new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _cityLogic.GetList();
            return View();
        }
    }
}