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
    public class OrderDocumentController : Controller
    {
        private Lms_OrderDocumentLogic _orderDocumentLogic;
        private readonly LogisticsContext _dbContext;

        public OrderDocumentController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _orderDocumentLogic = new Lms_OrderDocumentLogic(new EntityFrameworkGenericRepository<Lms_OrderDocumentPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _orderDocumentLogic.GetList();
            return View();
        }
    }
}