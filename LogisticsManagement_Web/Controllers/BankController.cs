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
    public class BankController : Controller
    {
        private Lms_BankLogic _bankLogic;
        private readonly LogisticsContext _dbContext;

        public BankController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _bankLogic = new Lms_BankLogic(new EntityFrameworkGenericRepository<Lms_BankPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _bankLogic.GetAllList();
            return View();
        }
    }
}