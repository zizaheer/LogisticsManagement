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
    public class AccTransactionDetailController : Controller
    {
        private Lms_AccTransactionDetailLogic _accTransactionDetailLogic;
        private readonly LogisticsContext _dbContext;

        public AccTransactionDetailController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _accTransactionDetailLogic = new Lms_AccTransactionDetailLogic(new EntityFrameworkGenericRepository<Lms_AccTransactionDetailPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _accTransactionDetailLogic.GetAllList();
            return View();
        }
    }
}