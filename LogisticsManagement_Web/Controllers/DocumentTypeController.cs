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
    public class DocumentTypeController : Controller
    {
        private App_DocumentTypeLogic _documentTypeLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public DocumentTypeController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _documentTypeLogic = new App_DocumentTypeLogic(_cache, new EntityFrameworkGenericRepository<App_DocumentTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _documentTypeLogic.GetList();
            return View();
        }
    }
}