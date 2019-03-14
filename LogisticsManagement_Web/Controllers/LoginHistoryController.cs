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
    public class LoginHistoryController : Controller
    {
        private App_LoginHistoryLogic _loginHistoryLogic;
        private readonly LogisticsContext _dbContext;

        public LoginHistoryController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _loginHistoryLogic = new App_LoginHistoryLogic(new EntityFrameworkGenericRepository<App_LoginHistoryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _loginHistoryLogic.GetAllList();
            return View();
        }
    }
}