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

        public IActionResult AddOrUpdate([FromBody]dynamic data)
        {

            return View();
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
            sessionData = JsonConvert.DeserializeObject<SessionData>(HttpContext.Session.GetString("SessionData"));
            if (sessionData == null)
            {
                Response.Redirect("Login/Index");
            }
        }
    }
}