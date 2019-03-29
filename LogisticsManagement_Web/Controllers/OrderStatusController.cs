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
    public class OrderStatusController : Controller
    {
        private Lms_OrderStatusLogic _orderStatusLogic;
        private readonly LogisticsContext _dbContext;

        public OrderStatusController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _orderStatusLogic = new Lms_OrderStatusLogic(new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _orderStatusLogic.GetList();
            return View();
        }
    }
}