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
    public class InvoiceStorageOrderMappingController : Controller
    {
        private Lms_InvoiceStorageOrderMappingLogic _invoiceStorageOrderMappingLogic;
        private readonly LogisticsContext _dbContext;

        public InvoiceStorageOrderMappingController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _invoiceStorageOrderMappingLogic = new Lms_InvoiceStorageOrderMappingLogic(new EntityFrameworkGenericRepository<Lms_InvoiceStorageOrderMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _invoiceStorageOrderMappingLogic.GetList();
            return View();
        }
    }
}