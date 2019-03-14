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
    public class AccountBalanceController : Controller
    {
        private Lms_AccountBalanceLogic _accountBalanceLogic;
        private readonly LogisticsContext _dbContext;

        public AccountBalanceController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _accountBalanceLogic = new Lms_AccountBalanceLogic(new EntityFrameworkGenericRepository<Lms_AccountBalancePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var balanceInfoList = _accountBalanceLogic.GetAllList();
            return View();
        }
    }
}