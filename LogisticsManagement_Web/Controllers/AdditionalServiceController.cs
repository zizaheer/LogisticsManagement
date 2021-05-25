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
        SessionData sessionData = new SessionData();

        public AdditionalServiceController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _additionalServiceLogic = new Lms_AdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var serviceList = _additionalServiceLogic.GetList();
            return View(serviceList);
        }

        public JsonResult GetAdditionalServiceList()
        {

            var serviceList = _additionalServiceLogic.GetList();
            if (serviceList != null)
            {
                return Json(JsonConvert.SerializeObject(serviceList.OrderBy(c => c.ServiceName)));
            }

            return Json("");

        }

        public JsonResult GetAdditionalServiceInfoById(string id)
        {
            if (id != "" && id != "undefined")
            {
                var serviceInfo = _additionalServiceLogic.GetList().Where(c => c.Id == Convert.ToInt32(id)).FirstOrDefault();
                return Json(JsonConvert.SerializeObject(serviceInfo));
            }

            return Json(string.Empty);
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            var serviceList = _additionalServiceLogic.GetList();
            return PartialView("_PartialViewServiceData", serviceList);
        }


        [HttpPost]
        public IActionResult Add([FromBody]dynamic serviceData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (serviceData != null)
                {
                    Lms_AdditionalServicePoco servicePoco = JsonConvert.DeserializeObject<Lms_AdditionalServicePoco>(JsonConvert.SerializeObject(serviceData));

                    if (servicePoco.Id < 1 && servicePoco.ServiceName.Trim() != string.Empty)
                    {
                        servicePoco.CreatedBy = sessionData.UserId;
                        var address = _additionalServiceLogic.Add(servicePoco);

                        result = "Success";
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Update([FromBody]dynamic serviceData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (serviceData != null)
                {
                    Lms_AdditionalServicePoco addressPoco = JsonConvert.DeserializeObject<Lms_AdditionalServicePoco>(JsonConvert.SerializeObject(serviceData));

                    if (addressPoco.Id > 0 && addressPoco.ServiceName.Trim() != string.Empty)
                    {
                        var existingService = _additionalServiceLogic.GetSingleById(addressPoco.Id);
                        // it is required to pull existing data first, 
                        // cause there are some data which do not come from UI

                        existingService.ServiceName = addressPoco.ServiceName;
                        existingService.ServiceCode = addressPoco.ServiceCode;
                        existingService.UnitPrice = addressPoco.UnitPrice;
                        existingService.IsPriceApplicable = addressPoco.IsPriceApplicable;
                        existingService.PayToDriver = addressPoco.PayToDriver;
                        existingService.IsTaxApplicable = addressPoco.IsTaxApplicable;
                        existingService.IsApplicableForStorage = addressPoco.IsApplicableForStorage;
                        existingService.IsActive = addressPoco.IsActive;

                        var poco = _additionalServiceLogic.Update(existingService);
                        result = poco.Id.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Remove(string id)
        {
            var result = "";
            try
            {
                var poco = _additionalServiceLogic.GetSingleById(Convert.ToInt32(id));
                if (poco != null)
                {
                    _additionalServiceLogic.Remove(poco);
                    result = "Success";
                }
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        private void ValidateSession()
        {
            if (HttpContext.Session.GetString("SessionData") != null)
            {
                sessionData = JsonConvert.DeserializeObject<SessionData>(HttpContext.Session.GetString("SessionData"));
                if (sessionData == null)
                {
                    Response.Redirect("Login/Index");
                }
            }
            else
            {
                Response.Redirect("Login/InvalidLocation");
            }
        }

    }
}