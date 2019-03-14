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
    public class AccountTypeController : Controller
    {
        private Lms_AccountTypeLogic _accountTypeLogic;
        private readonly LogisticsContext _dbContext;

        public AccountTypeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _accountTypeLogic = new Lms_AccountTypeLogic(new EntityFrameworkGenericRepository<Lms_AccountTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var typeList = _accountTypeLogic.GetAllList();
            return View();
        }
    }
}