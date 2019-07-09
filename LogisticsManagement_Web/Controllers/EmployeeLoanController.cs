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
    public class EmployeeLoanController : Controller
    {
        private Lms_EmployeeLoanLogic _employeeLoanLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public EmployeeLoanController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _employeeLoanLogic = new Lms_EmployeeLoanLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeLoanPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var employeeLoanList = _employeeLoanLogic.GetList();
            return View();
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic employeeLoanData)
        {

            ValidateSession();
            var result = "";

            try
            {
                if (employeeLoanData != null)
                {
                    Lms_EmployeeLoanPoco employeeLoanPoco = JsonConvert.DeserializeObject<Lms_EmployeeLoanPoco>(JsonConvert.SerializeObject(employeeLoanData[0]));

                    if (employeeLoanPoco.EmployeeId < 1 && employeeLoanPoco.LoanAmount>0)
                    {
                        using (var scope = new TransactionScope()) {

                            employeeLoanPoco.CreatedBy = sessionData.UserId;
                            var loanId = _employeeLoanLogic.Add(employeeLoanPoco);
                            if (!string.IsNullOrEmpty(loanId.ToString()))
                            {
                                result = loanId.ToString();
                                scope.Complete();
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
        public IActionResult Update([FromBody]dynamic employeeLoanData)
        {

            ValidateSession();
            var result = "";

            try
            {
                if (employeeLoanData != null)
                {
                    Lms_EmployeeLoanPoco employeeLoanPoco = JsonConvert.DeserializeObject<Lms_EmployeeLoanPoco>(JsonConvert.SerializeObject(employeeLoanData[0]));

                    if (employeeLoanPoco.EmployeeId < 1 && employeeLoanPoco.LoanAmount > 0)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var existingLoan = _employeeLoanLogic.GetList().Where(c => c.EmployeeId == employeeLoanPoco.EmployeeId).FirstOrDefault();
                            existingLoan.LoanAmount = employeeLoanPoco.LoanAmount;
                            existingLoan.LoanTakenOn = employeeLoanPoco.LoanTakenOn;
                            existingLoan.PaidAmount = employeeLoanPoco.PaidAmount;
                            existingLoan.Remarks = employeeLoanPoco.Remarks;

                            var loanId = _employeeLoanLogic.Update(existingLoan);
                            if (!string.IsNullOrEmpty(loanId.ToString()))
                            {
                                result = loanId.ToString();
                                scope.Complete();
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
        public IActionResult Remove(string id)
        {
            var result = "";
            try
            {
                var poco = _employeeLoanLogic.GetList().Where(c=>c.EmployeeId== Convert.ToInt32(id)).FirstOrDefault();
                _employeeLoanLogic.Remove(poco);

                result = "Success";
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