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
    public class BillPaymentController : Controller
    {
        private Lms_BillPaymentLogic _billPaymentLogic;
        private readonly LogisticsContext _dbContext;

        public BillPaymentController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _billPaymentLogic = new Lms_BillPaymentLogic(new EntityFrameworkGenericRepository<Lms_BillPaymentPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _billPaymentLogic.GetAllList();
            return View();
        }
    }
}