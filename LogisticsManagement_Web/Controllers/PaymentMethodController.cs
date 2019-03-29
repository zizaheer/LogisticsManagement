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
    public class PaymentMethodController : Controller
    {
        private Lms_PaymentMethodLogic _paymentMethodLogic;
        private readonly LogisticsContext _dbContext;

        public PaymentMethodController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _paymentMethodLogic = new Lms_PaymentMethodLogic(new EntityFrameworkGenericRepository<Lms_PaymentMethodPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _paymentMethodLogic.GetList();
            return View();
        }
    }
}