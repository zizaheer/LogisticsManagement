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
    public class PayeeController : Controller
    {
        private Lms_PayeeLogic _payeeLogic;

        private Lms_EmployeeLogic _employeeLogic;
        private Lms_EmployeeTypeLogic _employeeTypeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private Lms_CustomerAddressMappingLogic _customerAddressLogic;
        private Lms_ChartOfAccountLogic _chartOfAccountLogic;
        private Lms_ConfigurationLogic _configurationLogic;

        private readonly LogisticsContext _dbContext;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public PayeeController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _payeeLogic = new Lms_PayeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_PayeePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var payeeList = _payeeLogic.GetList();
            return View();
        }


        [HttpPost]
        public IActionResult Add([FromBody]dynamic payeeData)
        {

            ValidateSession();
            var result = "";

            try
            {
                if (payeeData != null)
                {
                    Lms_PayeePoco payeePoco = JsonConvert.DeserializeObject<Lms_PayeePoco>(JsonConvert.SerializeObject(payeeData[0]));

                    if (payeePoco.Id < 1 && payeePoco.PayeeName.Trim() != string.Empty)
                    {

                        _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                        _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));

                        var parentGLForBillAccount = _configurationLogic.GetSingleById(1).OtherPayableAccount;
                        var accounts = _chartOfAccountLogic.GetList().Where(c => c.ParentGLCode == parentGLForBillAccount).ToList();
                        var newAccountId = accounts.Max(c => c.Id) + 1;

                        using (var scope = new TransactionScope())
                        {
                            Lms_ChartOfAccountPoco accountPoco = new Lms_ChartOfAccountPoco();
                            accountPoco.Id = newAccountId;
                            accountPoco.ParentGLCode = parentGLForBillAccount;
                            accountPoco.AccountName = payeePoco.PayeeName;
                            accountPoco.BranchId = sessionData.BranchId == null ? 1 : (int)sessionData.BranchId;
                            accountPoco.CurrentBalance = 0;
                            accountPoco.IsActive = true;
                            accountPoco.Remarks = "Payee Account Payable";
                            accountPoco.CreateDate = DateTime.Now;
                            accountPoco.CreatedBy = sessionData.UserId;

                            var addedAcc = _chartOfAccountLogic.Add(accountPoco);
                            if (addedAcc.Id > 0)
                            {
                                payeePoco.AccountNo = addedAcc.Id.ToString();
                                var payeeId = _payeeLogic.Add(payeePoco).Id;

                                if (payeeId > 0)
                                {
                                    scope.Complete();
                                    result = payeeId.ToString();
                                }
                            }
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
        public IActionResult Update([FromBody]dynamic payeeData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (payeeData != null)
                {
                    Lms_PayeePoco payeePoco = JsonConvert.DeserializeObject<Lms_PayeePoco>(JsonConvert.SerializeObject(payeeData[0]));

                    if (payeePoco.Id > 0 && payeePoco.PayeeName.Trim() != string.Empty)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var existingPayee = _payeeLogic.GetSingleById(payeePoco.Id);
                            if (existingPayee != null)
                            {
                                existingPayee.PayeeName = payeePoco.PayeeName;
                                existingPayee.Address = payeePoco.Address;
                                existingPayee.EmailAddress = payeePoco.EmailAddress;
                                existingPayee.PhoneNumber = payeePoco.PhoneNumber;
                                _payeeLogic.Update(existingPayee);

                                result = existingPayee.Id.ToString();
                            }

                            scope.Complete();
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
                var poco = _payeeLogic.GetSingleById(Convert.ToInt32(id));
                _payeeLogic.Remove(poco);

                result = "Success";
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        public JsonResult GetPayeeById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var payee = _payeeLogic.GetSingleById(Convert.ToInt32(id));
                if (payee != null)
                {
                    return Json(JsonConvert.SerializeObject(payee));
                }
            }
            return Json(string.Empty);
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