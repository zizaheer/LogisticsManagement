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
    public class InvoiceStorageOrderMappingController : Controller
    {
        private Lms_InvoiceStorageOrderMappingLogic _invoiceStorageOrderMappingLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public InvoiceStorageOrderMappingController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _invoiceStorageOrderMappingLogic = new Lms_InvoiceStorageOrderMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoiceStorageOrderMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _invoiceStorageOrderMappingLogic.GetList();
            return View();
        }
    }
}