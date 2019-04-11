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
    public class AccTransactionDetailController : Controller
    {
        private Lms_AccTransactionDetailLogic _accTransactionDetailLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public AccTransactionDetailController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _accTransactionDetailLogic = new Lms_AccTransactionDetailLogic(_cache, new EntityFrameworkGenericRepository<Lms_AccTransactionDetailPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _accTransactionDetailLogic.GetList();
            return View();
        }
    }
}