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
    public class CustomerController : Controller
    {
        private Lms_CustomerLogic _customerLogic;
        private readonly LogisticsContext _dbContext;

        public CustomerController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _customerLogic = new Lms_CustomerLogic(new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _customerLogic.GetList();
            return View();
        }
    }
}