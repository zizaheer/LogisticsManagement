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
    public class InvoiceWayBillMappingController : Controller
    {
        private Lms_InvoiceWayBillMappingLogic _invoiceWayBillMappingLogic;
        private readonly LogisticsContext _dbContext;

        public InvoiceWayBillMappingController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _invoiceWayBillMappingLogic = new Lms_InvoiceWayBillMappingLogic(new EntityFrameworkGenericRepository<Lms_InvoiceWayBillMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _invoiceWayBillMappingLogic.GetList();
            return View();
        }
    }
}