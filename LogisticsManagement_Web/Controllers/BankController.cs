using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
    public class BankController : Controller
    {
        private Lms_BankLogic _bankLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public BankController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _bankLogic = new Lms_BankLogic(_cache, new EntityFrameworkGenericRepository<Lms_BankPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _bankLogic.GetList();
            ViewModel_Bank _bankList = new ViewModel_Bank();
            _bankList.Banks = customerList;
            return View(_bankList);
        }

        [HttpGet]
        public JsonResult GetAllBanks()
        {
            string result = "";
            try
            {
                var bankList = _bankLogic.GetList();
                if (bankList != null && bankList.Count > 0)
                {
                    result = JsonConvert.SerializeObject(bankList);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic bankData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (bankData != null)
                {
                   Lms_BankPoco bankPoco = JsonConvert.DeserializeObject<Lms_BankPoco>(JsonConvert.SerializeObject(bankData[0]));

                    using (var scope = new TransactionScope())
                    {
                        bankPoco.CreateDate = DateTime.Now;
                        bankPoco.CreatedBy = sessionData.UserId;

                        var bankId = _bankLogic.Add(bankPoco).Id;
                        if (bankId > 0)
                        {
                            scope.Complete();
                            result = bankId.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Update([FromBody]dynamic bankData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (bankData != null)
                {
                    Lms_BankPoco bankPoco = JsonConvert.DeserializeObject<Lms_BankPoco>(JsonConvert.SerializeObject(bankData[0]));

                    using (var scope = new TransactionScope())
                    {
                        var existingBank = _bankLogic.GetSingleById(bankPoco.Id);

                        existingBank.BankName = bankPoco.BankName;
                        var bankId = _bankLogic.Update(existingBank).Id;
                        if (bankId > 0)
                        {
                            scope.Complete();
                            result = bankId.ToString();
                        }
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
                var poco = _bankLogic.GetSingleById(Convert.ToInt32(id));
                if (poco != null)
                {
                    _bankLogic.Remove(poco);
                    result = "Success";
                }
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetBankById(string Id)
        {
            string result = "";
            try
            {
                var bankInfo = _bankLogic.GetSingleById(Convert.ToInt32(Id));
                if (bankInfo != null)
                {
                    result = JsonConvert.SerializeObject(bankInfo);
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