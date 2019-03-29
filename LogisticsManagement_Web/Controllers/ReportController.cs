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
    public class ReportController : Controller
    {
        private Lms_OrderLogic _orderLogic;
        private readonly LogisticsContext _dbContext;

        public ReportController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _orderLogic = new Lms_OrderLogic(new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _orderLogic.GetList();
            return View();
        }
    }
}