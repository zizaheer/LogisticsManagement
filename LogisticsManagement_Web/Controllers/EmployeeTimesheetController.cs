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
    public class EmployeeTimesheetController : Controller
    {
        private Lms_EmployeeTimesheetLogic _employeeTimesheetLogic;
        private readonly LogisticsContext _dbContext;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public EmployeeTimesheetController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _employeeTimesheetLogic = new Lms_EmployeeTimesheetLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeTimesheetPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            
            var employeeSignInList = _employeeTimesheetLogic.GetList().Where(c => c.SignInDatetime >= DateTime.Today.Date);
            return View(employeeSignInList);
        }


        [HttpPost]
        public IActionResult Add([FromBody]dynamic clockInData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (clockInData != null)
                {
                    var data = (JObject)JsonConvert.SerializeObject(clockInData[0]);

                    var empId = data.SelectToken("empId");

                    var currentDate = DateTime.Now.ToString("dd-MMM-yyyy");

                    using (var scope = new TransactionScope())
                    {
                        Lms_EmployeeTimesheetPoco timesheetPoco = new Lms_EmployeeTimesheetPoco();

                        timesheetPoco.CreateDate = DateTime.Now;
                        timesheetPoco.CreatedBy = sessionData.UserId;

                        var addedAcc = _employeeTimesheetLogic.Add(timesheetPoco);

                        scope.Complete();

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Update([FromBody]dynamic clockInData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (clockInData != null)
                {
                    var data = (JObject)JsonConvert.SerializeObject(clockInData);


                    using (var scope = new TransactionScope())
                    {
                        Lms_EmployeeTimesheetPoco timesheetPoco = new Lms_EmployeeTimesheetPoco();

                        timesheetPoco.CreateDate = DateTime.Now;
                        timesheetPoco.CreatedBy = sessionData.UserId;

                        var addedAcc = _employeeTimesheetLogic.Add(timesheetPoco);

                        scope.Complete();

                    }
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