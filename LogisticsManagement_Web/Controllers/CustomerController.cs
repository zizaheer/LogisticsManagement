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
    public class CustomerController : Controller
    {
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_EmployeeLogic _employeeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private readonly LogisticsContext _dbContext;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public CustomerController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;

            _dbContext = dbContext;
            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();
            return View(GetCustomerData());
        }


        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewCustomerData", GetCustomerData());
        }


        [HttpPost]
        public IActionResult AddOrUpdate([FromBody]dynamic customerData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (customerData != null)
                {
                    Lms_CustomerPoco customerPoco = JsonConvert.DeserializeObject<Lms_CustomerPoco>(JsonConvert.SerializeObject(customerData[0]));
                    Lms_AddressPoco billingAddressPoco = JsonConvert.DeserializeObject<Lms_AddressPoco>(JsonConvert.SerializeObject(customerData[1]));
                    Lms_AddressPoco mailingAddressPoco = JsonConvert.DeserializeObject<Lms_AddressPoco>(JsonConvert.SerializeObject(customerData[2]));

                    var addressData = GetCustomerData().Addressess;

                    if (customerPoco.Id > 0)
                    {
                        var customer = _customerLogic.GetSingleById(customerPoco.Id);
                        customer.Id = customerPoco.Id;
                        customer.CustomerName = customerPoco.CustomerName;
                        customer.DiscountPercentage = customerPoco.DiscountPercentage;
                        customer.InvoiceDueDays = customerPoco.InvoiceDueDays;
                        customer.IsGstApplicable = customerPoco.IsGstApplicable;
                        customer.BillingAddressId = customerPoco.BillingAddressId;
                        customer.MailingAddressId = customerPoco.MailingAddressId;
                        customer.IsActive = customerPoco.IsActive;

                        _customerLogic.Update(customer);
                    }
                    else
                    {
                        var customerId = _customerLogic.CreateNewCustomer(customerPoco, billingAddressPoco, mailingAddressPoco, (int)sessionData.BranchId);
                        if (!string.IsNullOrEmpty(customerId))
                        {
                            result = (JsonConvert.DeserializeObject<Lms_StoredProcedureResult>(customerId)).ReturnedValue;
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
            CustomerViewModel customerViewModel = new CustomerViewModel();
            customerViewModel.Customers = _customerLogic.GetList();

            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));

            customerViewModel.Addressess = _addressLogic.GetList();
            customerViewModel.Employees = _employeeLogic.GetList();
            customerViewModel.Cities = _cityLogic.GetList();
            customerViewModel.Provinces = _provinceLogic.GetList();
            customerViewModel.Countries = _countryLogic.GetList();

            return customerViewModel;
        }


        public JsonResult GetCustomers()
        {
            return Json(JsonConvert.SerializeObject(_customerLogic.GetList()));
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