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
    public class ScreenPermissionController : Controller
    {
        private App_ScreenPermissionLogic _screenPermissionLogic;
        private readonly LogisticsContext _dbContext;

        public ScreenPermissionController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _screenPermissionLogic = new App_ScreenPermissionLogic(new EntityFrameworkGenericRepository<App_ScreenPermissionPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _screenPermissionLogic.GetList();
            return View();
        }
    }
}