using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using System.Dynamic;

namespace LogisticsManagement_Web.Controllers
{
    public class OrderController : Controller
    {

        //private IMemoryCache _memoryCache;  // To do later 
        
        private Lms_OrderLogic _orderLogic;
        private readonly LogisticsContext _dbContext;

        public OrderController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _orderLogic = new Lms_OrderLogic(new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ViewBag.Tariffs = GetTariffs();
            ViewBag.DeliveryOptions = GetDeliveryOptions();
            //ViewBag.Customers = GetCustomers().OrderBy(c=>c.CustomerName);
            ViewBag.DeliveryTypes = Enum.GetValues(typeof(OrderType)).Cast<OrderType>();

            return View(ViewBag);
        }

        

        private List<Lms_TariffPoco> GetTariffs()
        {
            Lms_TariffLogic tariffLogic = new Lms_TariffLogic(new EntityFrameworkGenericRepository<Lms_TariffPoco>(_dbContext));
            return tariffLogic.GetAllList();
        }

        private List<Lms_DeliveryOptionPoco> GetDeliveryOptions()
        {
            Lms_DeliveryOptionLogic deliveryOptionLogic = new Lms_DeliveryOptionLogic(new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            return deliveryOptionLogic.GetAllList();
        }

        private List<Lms_CustomerPoco> GetCustomers()
        {
            Lms_CustomerLogic customerLogic = new Lms_CustomerLogic(new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            return customerLogic.GetAllList();
        }
       
    }
}