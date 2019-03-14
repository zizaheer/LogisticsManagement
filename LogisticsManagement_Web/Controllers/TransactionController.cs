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
    public class TransactionController : Controller
    {
        private Lms_TransactionLogic _transactionLogic;
        private readonly LogisticsContext _dbContext;

        public TransactionController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _transactionLogic = new Lms_TransactionLogic(new EntityFrameworkGenericRepository<Lms_TransactionPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _transactionLogic.GetAllList();
            return View();
        }
    }
}