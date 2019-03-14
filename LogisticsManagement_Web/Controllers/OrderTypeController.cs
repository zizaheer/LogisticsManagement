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
    public class OrderTypeController : Controller
    {
        private Lms_OrderTypeLogic _orderTypeLogic;
        private readonly LogisticsContext _dbContext;

        public OrderTypeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _orderTypeLogic = new Lms_OrderTypeLogic(new EntityFrameworkGenericRepository<Lms_OrderTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _orderTypeLogic.GetAllList();
            return View();
        }
    }
}