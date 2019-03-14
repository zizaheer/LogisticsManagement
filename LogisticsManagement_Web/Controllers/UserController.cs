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
    public class UserController : Controller
    {
        private App_UserLogic _userLogic;
        private readonly LogisticsContext _dbContext;

        public UserController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _userLogic = new App_UserLogic(new EntityFrameworkGenericRepository<App_UserPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _userLogic.GetAllList();
            return View();
        }
    }
}