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
    public class AdministrationController : Controller
    {
        private App_UserGroupLogic _userGroupLogic;
        private readonly LogisticsContext _dbContext;

        public AdministrationController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _userGroupLogic = new App_UserGroupLogic(new EntityFrameworkGenericRepository<App_UserGroupPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Customer controller operations
        /// </summary>
        /// <returns></returns>
        public IActionResult Customer()
        {
            var customerList = _userGroupLogic.GetAllList();
            return View();
        }

        public IActionResult Employee()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult AppUser()
        {
            return View();
        }

        public IActionResult AppUserGroup()
        {
            return View();
        }

        public IActionResult Payee()
        {
            return View();
        }

        public IActionResult BillPayment()
        {
            return View();
        }

        public IActionResult Tariff()
        {
            return View();
        }

        public IActionResult Configuration()
        {
            return View();
        }
    }
}