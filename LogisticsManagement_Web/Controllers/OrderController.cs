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
    public class OrderController : Controller
    {

        //private IMemoryCache _cache;  // To do later 
        
        private Lms_OrderLogic _orderLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private Lms_UnitTypeLogic _unitTypeLogic;
        private Lms_WeightScaleLogic _weightScaleLogic;
        private Lms_OrderAdditionalServiceLogic _orderAdditionalServiceLogic;
        private Lms_ConfigurationLogic _configurationLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public OrderController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();

            return View(GetOrderData());
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewOrderData", GetOrderData());
        }

        private DeliveryOrderViewModel GetOrderData()
        {
            DeliveryOrderViewModel deliveryOrderViewModel = new DeliveryOrderViewModel();
            deliveryOrderViewModel.Orders = _orderLogic.GetList();

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            deliveryOrderViewModel.Cities = _cityLogic.GetList();
            deliveryOrderViewModel.Provinces = _provinceLogic.GetList();

            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
            deliveryOrderViewModel.Configuration = _configurationLogic.GetList().FirstOrDefault();



            return deliveryOrderViewModel;
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
                        employeePoco.CreatedBy = sessionData.UserId;
                        var employeeId = _employeeLogic.CreateNewEmployee(employeePoco, (int)sessionData.BranchId);
                        if (!string.IsNullOrEmpty(employeeId))
                        {
                            var jObject = JObject.Parse(employeeId);
                            var returnedObject = (string)jObject.SelectToken("ReturnedValue");
                            result = (string)JObject.Parse(returnedObject).SelectToken("EmployeeId");
                            if (result.Length > 0)
                            {
                                result = Convert.ToInt32(result) < 1 ? "" : result;
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
            bool result = false;
            try
            {
                var poco = _employeeLogic.GetSingleById(Convert.ToInt32(id));
                _employeeLogic.Remove(poco);

                result = true;
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