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
    public class AdditionalServiceController : Controller
    {
        private Lms_AdditionalServiceLogic _additionalServiceLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public AdditionalServiceController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _additionalServiceLogic = new Lms_AdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _additionalServiceLogic.GetList();
            return View();
        }

        public JsonResult GetAdditionalServiceList()
        {

            var serviceList = _additionalServiceLogic.GetList();
            return Json(JsonConvert.SerializeObject(serviceList));

        }

        public JsonResult GetAdditionalServiceInfoById(string id)
        {
            if (id != "")
            {
                var serviceInfo = _additionalServiceLogic.GetList().Where(c => c.Id == Convert.ToInt32(id)).FirstOrDefault();
                return Json(JsonConvert.SerializeObject(serviceInfo));
            }

            return Json(string.Empty);
        }
    }
}