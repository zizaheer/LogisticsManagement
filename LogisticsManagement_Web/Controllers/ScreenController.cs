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
    public class ScreenController : Controller
    {
        private App_ScreenLogic _screenLogic;
        private readonly LogisticsContext _dbContext;

        public ScreenController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _screenLogic = new App_ScreenLogic(new EntityFrameworkGenericRepository<App_ScreenPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _screenLogic.GetAllList();
            return View();
        }
    }
}