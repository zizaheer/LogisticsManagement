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
    public class LoginHistoryController : Controller
    {
        private App_LoginHistoryLogic _loginHistoryLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public LoginHistoryController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _loginHistoryLogic = new App_LoginHistoryLogic(_cache, new EntityFrameworkGenericRepository<App_LoginHistoryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _loginHistoryLogic.GetList();
            return View();
        }
    }
}