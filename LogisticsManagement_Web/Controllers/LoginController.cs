using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LogisticsManagement_Web.Controllers
{
    public class LoginController : Controller
    {
        private App_UserLogic _userLogic;
        private readonly LogisticsContext _dbContext;

        public LoginController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _userLogic = new App_UserLogic(new EntityFrameworkGenericRepository<App_UserPoco>(_dbContext));
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string userName, string userPassword)
        {
            var userData = _userLogic.GetAllList().Where(c => c.UserName == userName && c.Password == userPassword).ToList();
            if (userData != null && userData.Count > 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index");
        }
    }
}