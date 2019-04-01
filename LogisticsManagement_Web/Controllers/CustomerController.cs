using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LogisticsManagement_Web.Controllers
{
    public class CustomerController : Controller
    {
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_EmployeeLogic _employeeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private readonly LogisticsContext _dbContext;

        public CustomerController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _customerLogic = new Lms_CustomerLogic(new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            return View(GetCustomerData());
        }


        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            return PartialView("_PartialViewCustomerData", GetCustomerData());
        }


        [HttpPost]
        public IActionResult AddOrUpdate([FromBody]dynamic customerData)
        {
            var result = false;
            try
            {
                var serializedData = JsonConvert.SerializeObject(customerData);
                Lms_CustomerPoco[] pocos = JsonConvert.DeserializeObject<Lms_CustomerPoco[]>(serializedData);

                pocos.FirstOrDefault().CreateDate = DateTime.Now;
                pocos.FirstOrDefault().CreatedBy = 1;
                if (pocos.FirstOrDefault().Id > 0)
                {
                    _customerLogic.Update(pocos);
                }
                else
                {
                    _customerLogic.Add(pocos);
                }

                result = true;
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Remove([FromBody]dynamic customerData)
        {
            bool result = false;
            try
            {
                var serializedData = JsonConvert.SerializeObject(customerData);
                Lms_CustomerPoco[] pocos = JsonConvert.DeserializeObject<Lms_CustomerPoco[]>(serializedData);

                _customerLogic.Remove(pocos);
                result = true;
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        private CustomerViewModel GetCustomerData()
        {
            _cityLogic = new App_CityLogic(new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _employeeLogic = new Lms_EmployeeLogic(new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            _addressLogic = new Lms_AddressLogic(new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));

            CustomerViewModel customerViewModel = new CustomerViewModel();

            customerViewModel.Customers = _customerLogic.GetList();
            customerViewModel.Employees = _employeeLogic.GetList();
            customerViewModel.Addressess = _addressLogic.GetList();
            customerViewModel.Cities = _cityLogic.GetList();
            customerViewModel.Provinces = _provinceLogic.GetList();
            customerViewModel.Countries = _countryLogic.GetList();

            return customerViewModel;
        }

    }
}