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
    public class EmployeeController : Controller
    {
        private Lms_EmployeeLogic _employeeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public EmployeeController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();

            ViewBag.EmployeeTypes = Enum.GetValues(typeof(EmployeeType)).Cast<EmployeeType>();

            return View(GetEmployeeData());

        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewEmployeeData", GetEmployeeData());
        }

        private EmployeeViewModel GetEmployeeData()
        {
            EmployeeViewModel employeeViewModel = new EmployeeViewModel();
            employeeViewModel.Employees = _employeeLogic.GetList();

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));

            employeeViewModel.Cities = _cityLogic.GetList();
            employeeViewModel.Provinces = _provinceLogic.GetList();
            employeeViewModel.Countries = _countryLogic.GetList();

            return employeeViewModel;
        }

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
                        employeePoco.CreatedBy = sessionData.UserId;
                        var employeeId = _employeeLogic.CreateNewEmployee(employeePoco, (int)sessionData.BranchId);
                        if (!string.IsNullOrEmpty(employeeId))
                        {
                            var jObject = JObject.Parse(employeeId);
                            var returnedObject = (string)jObject.SelectToken("ReturnedValue");
                            result = (string)JObject.Parse(returnedObject).SelectToken("EmployeeId");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

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


                        _employeeLogic.Update(employee);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }


        public JsonResult GetEmployees()
        {
            var employeeList = _employeeLogic.GetList().OrderBy(c => c.FirstName);
            return Json(JsonConvert.SerializeObject(employeeList));
        }

        public JsonResult GetEmployeeById(string id)
        {
            var employeeList = _employeeLogic.GetList();
            return Json(JsonConvert.SerializeObject(employeeList));
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