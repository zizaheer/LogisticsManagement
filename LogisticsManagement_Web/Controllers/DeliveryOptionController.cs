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
    public class DeliveryOptionController : Controller
    {
        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private readonly LogisticsContext _dbContext;

        public DeliveryOptionController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _deliveryOptionLogic.GetAllList();
            return View();
        }
    }
}