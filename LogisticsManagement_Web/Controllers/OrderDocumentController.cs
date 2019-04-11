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
    public class OrderDocumentController : Controller
    {
        private Lms_OrderDocumentLogic _orderDocumentLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public OrderDocumentController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _orderDocumentLogic = new Lms_OrderDocumentLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderDocumentPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _orderDocumentLogic.GetList();
            return View();
        }
    }
}