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
            ValidateSession();
            var employeeSignInList = _employeeTimesheetLogic.GetList().Where(c => c.SignInDatetime >= DateTime.Today.Date);
            return View(employeeSignInList);
        }


        [HttpGet]
        public IActionResult GetEmployeeClockInInfo(string id)
        {
            var result = "";
            ValidateSession();

            try
            {
                var currentEmployeeTimesheet = _employeeTimesheetLogic.GetList().Where(c => c.EmployeeId == sessionData.LoggedInEmployeeId).FirstOrDefault();
                if (currentEmployeeTimesheet != null)
                {
                    return Json(JsonConvert.SerializeObject(currentEmployeeTimesheet));
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
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
                    var data = JsonConvert.SerializeObject(clockInData[0]);
                    //var clockInTime = JObject.Parse(data)["clockInTime"].ToString();
                    //var clockOutTime = JObject.Parse(data)["clockOutTime"].ToString();
                    var remarks = JObject.Parse(data)["remarks"].ToString();
                    var breakTime = JObject.Parse(data)["breakTime"].ToString();

                    var empId = sessionData.LoggedInEmployeeId;

                    var currentDate = DateTime.Now.ToString("dd-MMM-yyyy");

                    using (var scope = new TransactionScope())
                    {
                        var todaysTimeSheetInfo = _employeeTimesheetLogic.GetList().Where(c => c.DateWorked.ToString("dd-MMM-yyyy") == currentDate && c.EmployeeId == empId).FirstOrDefault();

                        if (todaysTimeSheetInfo != null)
                        {
                            result = "";
                        }
                        else
                        {
                            Lms_EmployeeTimesheetPoco timesheetPoco = new Lms_EmployeeTimesheetPoco();

                            timesheetPoco.EmployeeId = (int)empId;
                            timesheetPoco.SignInDatetime = DateTime.Now;
                            timesheetPoco.DateWorked = Convert.ToDateTime(currentDate);
                            timesheetPoco.Remarks = remarks;
                            timesheetPoco.CreateDate = DateTime.Now;
                            timesheetPoco.CreatedBy = sessionData.UserId;

                            _employeeTimesheetLogic.Add(timesheetPoco);

                            result = "Success";
                        }

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
                    var data = JsonConvert.SerializeObject(clockInData[0]);
                    //var clockInTime = JObject.Parse(data)["clockInTime"].ToString();
                    //var clockOutTime = JObject.Parse(data)["clockOutTime"].ToString();
                    var remarks = JObject.Parse(data)["remarks"].ToString();
                    var breakTime = JObject.Parse(data)["breakTime"].ToString();

                    var empId = sessionData.LoggedInEmployeeId;
                    var currentDate = DateTime.Now.ToString("dd-MMM-yyyy");

                    using (var scope = new TransactionScope())
                    {
                        var todaysTimeSheetInfo = _employeeTimesheetLogic.GetList().Where(c => c.DateWorked.ToString("dd-MMM-yyyy") == currentDate && c.EmployeeId == empId).FirstOrDefault();

                        if (todaysTimeSheetInfo != null)
                        {
                            todaysTimeSheetInfo.SignOutDatetime = DateTime.Now;
                            todaysTimeSheetInfo.Remarks = remarks;
                            todaysTimeSheetInfo.BreakTime = !string.IsNullOrEmpty(breakTime.ToString()) ? Convert.ToDecimal(breakTime) : 0;

                            _employeeTimesheetLogic.Update(todaysTimeSheetInfo);

                            result = "Success";
                        }
                        else
                        {
                            result = "";
                        }
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