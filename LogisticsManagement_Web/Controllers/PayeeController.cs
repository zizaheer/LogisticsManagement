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
    public class PayeeController : Controller
    {
        private Lms_PayeeLogic _payeeLogic;
        private readonly LogisticsContext _dbContext;

        public PayeeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _payeeLogic = new Lms_PayeeLogic(new EntityFrameworkGenericRepository<Lms_PayeePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _payeeLogic.GetAllList();
            return View();
        }
    }
}