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
    public class EmployeeController : Controller
    {
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

        public EmployeeController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            _employeeTypeLogic = new Lms_EmployeeTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();

            ViewBag.EmployeeTypes = _employeeTypeLogic.GetList();

            return View(GetEmployeeData());

        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewEmployeeData", GetEmployeeData());
        }

        [HttpGet]
        public IActionResult GetEmployeeList()
        {
            ValidateSession();
            var employeeList = _employeeLogic.GetList();
            if (employeeList.Count > 0) {

                return Json(JsonConvert.SerializeObject(employeeList));
            }
            return Json("");
        }


        public ViewModel_Employee GetEmployeeData()
        {
            ViewModel_Employee employeeViewModel = new ViewModel_Employee();
            employeeViewModel.Employees = _employeeLogic.GetList();
            employeeViewModel.EmployeeTypes = _employeeTypeLogic.GetList();

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));

            employeeViewModel.Cities = _cityLogic.GetList();
            employeeViewModel.Provinces = _provinceLogic.GetList();
            employeeViewModel.Countries = _countryLogic.GetList();

            return employeeViewModel;
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic employeeData)
        {

            ValidateSession();
            var result = "";

            try
            {
                if (employeeData != null)
                {
                    Lms_EmployeePoco employeePoco = JsonConvert.DeserializeObject<Lms_EmployeePoco>(JsonConvert.SerializeObject(employeeData[0]));

                    if (employeePoco.Id < 1 && employeePoco.FirstName.Trim() != string.Empty)
                    {

                        _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                        _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));

                        var parentGLForEmployeeAccount = _configurationLogic.GetSingleById(1).ParentGLForEmployeeAccount;
                        var accounts = _chartOfAccountLogic.GetList().Where(c => c.ParentGLCode == parentGLForEmployeeAccount).ToList();
                        var newAccountId = accounts.Max(c => c.Id) + 1;
                        var newEmployeeId = _employeeLogic.GetMaxId() + 1;

                        using (var scope = new TransactionScope())
                        {
                            Lms_ChartOfAccountPoco accountPoco = new Lms_ChartOfAccountPoco();
                            accountPoco.Id = newAccountId;
                            accountPoco.ParentGLCode = parentGLForEmployeeAccount;
                            accountPoco.AccountName = employeePoco.FirstName + employeePoco.LastName;
                            accountPoco.BranchId = sessionData.BranchId == null ? 1 : (int)sessionData.BranchId;
                            accountPoco.CurrentBalance = 0;
                            accountPoco.IsActive = true;
                            accountPoco.Remarks = "Employee Account Payable";
                            accountPoco.CreateDate = DateTime.Now;
                            accountPoco.CreatedBy = sessionData.UserId;

                            var addedAcc = _chartOfAccountLogic.Add(accountPoco);
                            if (addedAcc.Id > 0)
                            {
                                employeePoco.Id = newEmployeeId;
                                employeePoco.AccountId = addedAcc.Id;
                                employeePoco.CreateDate = DateTime.Now;
                                employeePoco.CreatedBy = sessionData.UserId;
                                var employeeId = _employeeLogic.Add(employeePoco).Id;

                                if (employeeId > 0)
                                {
                                    scope.Complete();
                                    result = employeeId.ToString();
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
        public IActionResult Update([FromBody]dynamic employeeData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (employeeData != null)
                {
                    Lms_EmployeePoco employeePoco = JsonConvert.DeserializeObject<Lms_EmployeePoco>(JsonConvert.SerializeObject(employeeData[0]));

                    if (employeePoco.Id > 0 && employeePoco.FirstName.Trim() != string.Empty)
                    {
                        var employee = _employeeLogic.GetSingleById(employeePoco.Id);
                        // it is required to pull existing data first, 
                        // cause there are some data which do not come from UI

                        employee.FirstName = employeePoco.FirstName;
                        employee.LastName = employeePoco.LastName;
                        employee.DriverLicenseNo = employeePoco.DriverLicenseNo;
                        employee.SocialInsuranceNo = employeePoco.SocialInsuranceNo;
                        employee.EmployeeTypeId = employeePoco.EmployeeTypeId;
                        employee.IsHourlyPaid = employeePoco.IsHourlyPaid;
                        employee.HourlyRate = employeePoco.HourlyRate;

                        employee.IsSalaried = employeePoco.IsSalaried;
                        employee.SalaryAmount = employeePoco.SalaryAmount;
                        employee.IsCommissionProvided = employeePoco.IsCommissionProvided;
                        employee.CommissionPercentage = employeePoco.CommissionPercentage;
                        employee.IsFuelChargeProvided = employeePoco.IsFuelChargeProvided;
                        employee.FuelPercentage = employeePoco.FuelPercentage;
                        employee.RadioInsuranceAmount = employeePoco.RadioInsuranceAmount;
                        employee.InsuranceAmount = employeePoco.InsuranceAmount;
                        employee.SalaryTerm = employeePoco.SalaryTerm;
                        employee.IsActive = employeePoco.IsActive;

                        employee.UnitNumber = employeePoco.UnitNumber;
                        employee.AddressLine = employeePoco.AddressLine;
                        employee.CityId = employeePoco.CityId;
                        employee.ProvinceId = employeePoco.ProvinceId;
                        employee.CountryId = employeePoco.CountryId;
                        employee.PostCode = employeePoco.PostCode;
                        employee.EmailAddress = employeePoco.EmailAddress;
                        employee.MobileNumber = employeePoco.MobileNumber;
                        employee.FaxNumber = employeePoco.FaxNumber;
                        employee.PhoneNumber = employeePoco.PhoneNumber;


                        var poco = _employeeLogic.Update(employee);
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
                var poco = _employeeLogic.GetSingleById(Convert.ToInt32(id));
                _employeeLogic.Remove(poco);

                result = "Success";
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        public JsonResult GetEmployeeById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var employee = _employeeLogic.GetSingleById(Convert.ToInt32(id));
                if (employee != null) {
                    return Json(JsonConvert.SerializeObject(employee));
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