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
    public class InvoiceController : Controller
    {
        private Lms_InvoiceLogic _invoiceLogic;
        private readonly LogisticsContext _dbContext;

        public InvoiceController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _invoiceLogic = new Lms_InvoiceLogic(new EntityFrameworkGenericRepository<Lms_InvoicePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _invoiceLogic.GetList();
            return View();
        }
    }
}