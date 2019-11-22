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
    public class BillPaymentController : Controller
    {
        private Lms_BillPaymentLogic _billPaymentLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData;

        public BillPaymentController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _billPaymentLogic = new Lms_BillPaymentLogic(_cache, new EntityFrameworkGenericRepository<Lms_BillPaymentPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var billPaymentList = _billPaymentLogic.GetList();
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
                    Lms_BillPaymentPoco billPaymentPoco = JsonConvert.DeserializeObject<Lms_BillPaymentPoco>(JsonConvert.SerializeObject(employeeLoanData[0]));

                    if (billPaymentPoco.PaidAmount > 0)
                    {
                        using (var scope = new TransactionScope())
                        {

                            //var _transactionController = new TransactionController(_cache, _dbContext);
                            //var _configLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                            //var configInfo = _configLogic.GetSingleById(1);
                            //var employeeInfo = _employeeLogic.GetSingleById(billPaymentPoco.EmployeeId);

                            //List<TransactionModel> debitTransactionModelList = new List<TransactionModel>();
                            //TransactionModel debitTransactionModel = new TransactionModel();
                            //debitTransactionModel.AccountId = employeeInfo.AccountId;
                            //debitTransactionModel.TxnAmount = billPaymentPoco.LoanAmount;
                            //debitTransactionModelList.Add(debitTransactionModel);

                            //List<TransactionModel> creditTransactionModelList = new List<TransactionModel>();
                            //TransactionModel creditTransactionModel = new TransactionModel();
                            //creditTransactionModel.AccountId = (int)configInfo.LoanIncomeAccount;
                            //creditTransactionModel.TxnAmount = billPaymentPoco.LoanAmount;
                            //creditTransactionModelList.Add(creditTransactionModel);

                            //var txnId = _transactionController.MakeTransaction(debitTransactionModelList, creditTransactionModelList, billPaymentPoco.LoanAmount, DateTime.Now, DateTime.Now, billPaymentPoco.Remarks);

                            //billPaymentPoco.TransactionId = txnId;
                            //billPaymentPoco.CreatedBy = sessionData.UserId;
                            //var loanId = _employeeLoanLogic.Add(billPaymentPoco);
                            //if (!string.IsNullOrEmpty(loanId.ToString()))
                            //{
                            //    result = loanId.ToString();
                            //    scope.Complete();
                            //}
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
                            //var existingLoan = _employeeLoanLogic.GetList().Where(c => c.EmployeeId == employeeLoanPoco.EmployeeId).FirstOrDefault();
                            //existingLoan.LoanAmount = employeeLoanPoco.LoanAmount;
                            //existingLoan.LoanTakenOn = employeeLoanPoco.LoanTakenOn;
                            //existingLoan.Remarks = employeeLoanPoco.Remarks;

                            //var loanId = _employeeLoanLogic.Update(existingLoan);
                            //if (!string.IsNullOrEmpty(loanId.ToString()))
                            //{
                            //    result = loanId.ToString();
                            //    scope.Complete();
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpGet]
        public IActionResult GetLoanInfoByLoanId(string id)
        {
            try
            {
                //var employeeLoanInfo = _employeeLoanLogic.GetSingleById(Convert.ToInt32(id));
                //if (employeeLoanInfo != null)
                //{
                //    return Json(JsonConvert.SerializeObject(employeeLoanInfo));
                //}
            }

            catch (Exception ex)
            {

            }
            return Json("");
        }


        [HttpGet]
        public IActionResult PartialViewEmployeeLoans()
        {
            ValidateSession();
            return PartialView("_PartialViewEmployeeLoans", GetEmployeeLoans());
        }

        private List<ViewModel_EmployeeLoan> GetEmployeeLoans()
        {
            List<ViewModel_EmployeeLoan> employeeLoanViewList = new List<ViewModel_EmployeeLoan>();
            //var employeeLoanList = _employeeLoanLogic.GetList();
            //var employeeList = _employeeLogic.GetList();

            //foreach (var loan in employeeLoanList)
            //{
            //    ViewModel_EmployeeLoan employeeLoanView = new ViewModel_EmployeeLoan();
            //    employeeLoanView.LoanId = loan.Id;
            //    employeeLoanView.EmployeeId = loan.EmployeeId;
            //    employeeLoanView.EmployeeName = employeeList.Where(c => c.Id == loan.EmployeeId).FirstOrDefault().FirstName + " " + employeeList.Where(c => c.Id == loan.EmployeeId).FirstOrDefault().LastName;
            //    employeeLoanView.LoanAmount = loan.LoanAmount;
            //    employeeLoanView.LoanTakenOn = loan.LoanTakenOn;
            //    employeeLoanView.Remarks = loan.Remarks;

            //    employeeLoanViewList.Add(employeeLoanView);
            //}

            return employeeLoanViewList;
        }

        [HttpPost]
        public IActionResult Remove(string id)
        {
            var result = "";
            try
            {
                //var loanPoco = _employeeLoanLogic.GetSingleById(Convert.ToInt32(id));
                //if (loanPoco != null)
                //{
                //    TransactionController transactionController = new TransactionController(_cache, _dbContext);
                //    transactionController.RemoveTransaction((int)loanPoco.TransactionId);

                //    _employeeLoanLogic.Remove(loanPoco);
                //}

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