using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogisticsManagement_Web.Controllers
{
    public class OrderController : Controller
    {

        //private IMemoryCache _memoryCache;  // To do later 
        
        private Lms_OrderLogic _orderLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public OrderController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _orderLogic = new Lms_OrderLogic(new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();

            ViewBag.Tariffs = GetTariffs();
            ViewBag.DeliveryOptions = GetDeliveryOptions();
            //ViewBag.Customers = GetCustomers().OrderBy(c=>c.CustomerName);
            ViewBag.DeliveryTypes = Enum.GetValues(typeof(OrderType)).Cast<OrderType>();

            return View(ViewBag);
        }

        

        private List<Lms_TariffPoco> GetTariffs()
        {
            Lms_TariffLogic tariffLogic = new Lms_TariffLogic(new EntityFrameworkGenericRepository<Lms_TariffPoco>(_dbContext));
            return tariffLogic.GetList();
        }

        private List<Lms_DeliveryOptionPoco> GetDeliveryOptions()
        {
            Lms_DeliveryOptionLogic deliveryOptionLogic = new Lms_DeliveryOptionLogic(new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            return deliveryOptionLogic.GetList();
        }

        private List<Lms_CustomerPoco> GetCustomers()
        {
            Lms_CustomerLogic customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            return customerLogic.GetList();
        }

        private void ValidateSession()
        {
            sessionData = JsonConvert.DeserializeObject<SessionData>(HttpContext.Session.GetString("SessionData"));
            if (sessionData == null)
            {
                Response.Redirect("Login/Index");
            }
        }
    }
}