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
        private Lms_CustomerAddressMappingLogic _customerAddressMappingLogic;
        private Lms_ChartOfAccountLogic _chartOfAccountLogic;

        private Lms_AddressLogic _addressLogic;
        private Lms_EmployeeLogic _employeeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private Lms_ConfigurationLogic _configurationLogic;
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
            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
            ViewBag.DefaultFuelSurcharge = _configurationLogic.GetSingleById(1).DefaultFuelSurcharge;

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            ViewBag.Cities = _cityLogic.GetList();
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            ViewBag.Provinces = _provinceLogic.GetList();
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
            ViewBag.Countries = _countryLogic.GetList();

            return View(GetCustomerData(0));
        }

        [HttpGet]
        public IActionResult LoadCustomerData(string id)
        {
            ValidateSession();
            return PartialView("_PartialViewCustomerData", GetCustomerData(Convert.ToInt32(id)));
        }

        [HttpGet]
        public IActionResult LoadCustomerAddressData(string id)
        {
            ValidateSession();
            return PartialView("_PartialViewCustomerAddress", GetCustomerAddressData(Convert.ToInt32(id)));
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic customerData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (customerData != null)
                {
                    Lms_CustomerPoco customerPoco = JsonConvert.DeserializeObject<Lms_CustomerPoco>(JsonConvert.SerializeObject(customerData[0]));

                    CustomerAddressMapping customerAddress = JsonConvert.DeserializeObject<CustomerAddressMapping>(JsonConvert.SerializeObject(customerData[1]));

                    _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                    _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));

                    var parentGLForCustomerAccount = _configurationLogic.GetSingleById(1).ParentGLForCustomerAccount;
                    var accounts = _chartOfAccountLogic.GetList().Where(c => c.ParentGLCode == parentGLForCustomerAccount).ToList();
                    var newAccountId = accounts.Max(c => c.Id) + 1;
                    var newCustomerId = _customerLogic.GetMaxId() + 1;

                    using (var scope = new TransactionScope())
                    {
                        Lms_ChartOfAccountPoco accountPoco = new Lms_ChartOfAccountPoco();
                        accountPoco.Id = newAccountId;
                        accountPoco.ParentGLCode = parentGLForCustomerAccount;
                        accountPoco.AccountName = customerPoco.CustomerName;
                        accountPoco.BranchId = sessionData.BranchId == null ? 1 : (int)sessionData.BranchId;
                        accountPoco.CurrentBalance = 0;
                        accountPoco.IsActive = true;
                        accountPoco.Remarks = "Customer Account Receivable";
                        accountPoco.CreateDate = DateTime.Now;
                        accountPoco.CreatedBy = sessionData.UserId;

                        var addedAcc = _chartOfAccountLogic.Add(accountPoco);
                        if (addedAcc.Id > 0)
                        {
                            customerPoco.Id = newCustomerId;
                            customerPoco.AccountId = addedAcc.Id;
                            customerPoco.CreateDate = DateTime.Now;
                            customerPoco.CreatedBy = sessionData.UserId;
                            var customerId = _customerLogic.Add(customerPoco).Id;

                            result = customerId.ToString();
                        }

                        if (customerAddress != null && !string.IsNullOrEmpty(customerAddress.AddressLine))
                        {
                            customerAddress.CustomerId = newCustomerId;
                            customerAddress.IsDefault = true;
                            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                            var addressList = _addressLogic.GetList();

                            _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
                            var customerAddressList = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == customerAddress.CustomerId);

                            int addressId = 0;
                            var existingAddress = addressList.Where(c => c.UnitNumber == customerAddress.UnitNumber && c.AddressLine == customerAddress.AddressLine && c.CityId == customerAddress.CityId).FirstOrDefault();
                            if (existingAddress != null)
                            {
                                existingAddress.ProvinceId = customerAddress.ProvinceId;
                                existingAddress.CountryId = customerAddress.CountryId;
                                existingAddress.PostCode = customerAddress.PostCode;
                                existingAddress.PrimaryPhoneNumber = customerAddress.PrimaryPhoneNumber;
                                existingAddress.Fax = customerAddress.Fax;
                                existingAddress.EmailAddress1 = customerAddress.EmailAddress1;
                                existingAddress.EmailAddress2 = customerAddress.EmailAddress1;
                                existingAddress.ContactPersonName = customerAddress.ContactPersonName;

                                addressId = _addressLogic.Update(existingAddress).Id;
                            }
                            else
                            {
                                Lms_AddressPoco addressPoco = new Lms_AddressPoco();
                                addressPoco.UnitNumber = customerAddress.UnitNumber;
                                addressPoco.AddressLine = customerAddress.AddressLine;
                                addressPoco.CityId = customerAddress.CityId;
                                addressPoco.ProvinceId = customerAddress.ProvinceId;
                                addressPoco.CountryId = customerAddress.CountryId;
                                addressPoco.PostCode = customerAddress.PostCode;
                                addressPoco.PrimaryPhoneNumber = customerAddress.PrimaryPhoneNumber;
                                addressPoco.Fax = customerAddress.Fax;
                                addressPoco.EmailAddress1 = customerAddress.EmailAddress1;
                                addressPoco.EmailAddress2 = customerAddress.EmailAddress1;
                                addressPoco.ContactPersonName = customerAddress.ContactPersonName;

                                addressId = _addressLogic.Add(addressPoco).Id;
                            }

                            if (customerAddress.AddressTypeId == 0)
                            {
                                Lms_CustomerAddressMappingPoco customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();
                                customerAddressMappingPoco.CustomerId = customerAddress.CustomerId;
                                customerAddressMappingPoco.AddressId = addressId;
                                customerAddressMappingPoco.AddressTypeId = (byte)Enum_AddressType.Billing;
                                customerAddressMappingPoco.IsDefault = customerAddress.IsDefault;
                                _customerAddressMappingLogic.Add(customerAddressMappingPoco);

                                customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();
                                customerAddressMappingPoco.CustomerId = customerAddress.CustomerId;
                                customerAddressMappingPoco.AddressId = addressId;
                                customerAddressMappingPoco.AddressTypeId = (byte)Enum_AddressType.Shipping;
                                customerAddressMappingPoco.IsDefault = customerAddress.IsDefault;
                                _customerAddressMappingLogic.Add(customerAddressMappingPoco);
                            }
                            else
                            {
                                Lms_CustomerAddressMappingPoco customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();
                                customerAddressMappingPoco.CustomerId = customerAddress.CustomerId;
                                customerAddressMappingPoco.AddressId = addressId;
                                customerAddressMappingPoco.AddressTypeId = customerAddress.AddressTypeId;
                                customerAddressMappingPoco.IsDefault = customerAddress.IsDefault;
                                _customerAddressMappingLogic.Add(customerAddressMappingPoco);
                            }

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
        public IActionResult Update([FromBody]dynamic customerData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (customerData != null)
                {
                    Lms_CustomerPoco customerPoco = JsonConvert.DeserializeObject<Lms_CustomerPoco>(JsonConvert.SerializeObject(customerData[0]));
                    CustomerAddressMapping customerAddress = JsonConvert.DeserializeObject<CustomerAddressMapping>(JsonConvert.SerializeObject(customerData[1]));
                    _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                    _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));
                    var jAddressObject = (JObject)customerData[1];
                    var shippingAddressMappingId = Convert.ToString(jAddressObject.SelectToken("shippingAddressMappingId"));
                    var billingAddressMappingId = Convert.ToString(jAddressObject.SelectToken("billingAddressMappingId"));
                    //using (var scope = new TransactionScope())
                    //{
                    if (customerPoco.Id > 0)
                    {
                        var customer = _customerLogic.GetSingleById(customerPoco.Id);
                        customer.CustomerName = customerPoco.CustomerName;
                        customer.DiscountPercentage = customerPoco.DiscountPercentage;
                        customer.FuelSurChargePercentage = customerPoco.FuelSurChargePercentage > 0 ? customerPoco.FuelSurChargePercentage : null;
                        customer.InvoiceDueDays = customerPoco.InvoiceDueDays;
                        customer.IsGstApplicable = customerPoco.IsGstApplicable;
                        customer.IsActive = customerPoco.IsActive;

                        var accountInfo = _chartOfAccountLogic.GetSingleById(customer.AccountId);
                        accountInfo.AccountName = customerPoco.CustomerName;

                        _chartOfAccountLogic.Update(accountInfo);
                        result = _customerLogic.Update(customer).Id.ToString();

                        if (customerAddress != null)
                        {
                            customerAddress.CustomerId = customerPoco.Id;
                            customerAddress.IsDefault = true;
                            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                            var addressList = _addressLogic.GetList();

                            _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
                            var customerAddressList = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == customerAddress.CustomerId);

                            int addressId = 0;
                            var existingAddress = addressList.Where(c => c.UnitNumber == customerAddress.UnitNumber && c.AddressLine == customerAddress.AddressLine && c.CityId == customerAddress.CityId).ToList().FirstOrDefault();
                            if (existingAddress != null)
                            {
                                existingAddress.ProvinceId = customerAddress.ProvinceId;
                                existingAddress.CountryId = customerAddress.CountryId;
                                existingAddress.PostCode = customerAddress.PostCode;
                                existingAddress.PrimaryPhoneNumber = customerAddress.PrimaryPhoneNumber;
                                existingAddress.Fax = customerAddress.Fax;
                                existingAddress.EmailAddress1 = customerAddress.EmailAddress1;
                                existingAddress.EmailAddress2 = customerAddress.EmailAddress1;
                                existingAddress.ContactPersonName = customerAddress.ContactPersonName;

                                addressId = _addressLogic.Update(existingAddress).Id;
                            }
                            else
                            {
                                Lms_AddressPoco addressPoco = new Lms_AddressPoco();
                                addressPoco.UnitNumber = customerAddress.UnitNumber;
                                addressPoco.AddressLine = customerAddress.AddressLine;
                                addressPoco.CityId = customerAddress.CityId;
                                addressPoco.ProvinceId = customerAddress.ProvinceId;
                                addressPoco.CountryId = customerAddress.CountryId;
                                addressPoco.PostCode = customerAddress.PostCode;
                                addressPoco.PrimaryPhoneNumber = customerAddress.PrimaryPhoneNumber;
                                addressPoco.Fax = customerAddress.Fax;
                                addressPoco.EmailAddress1 = customerAddress.EmailAddress1;
                                addressPoco.EmailAddress2 = customerAddress.EmailAddress1;
                                addressPoco.ContactPersonName = customerAddress.ContactPersonName;

                                addressId = _addressLogic.Add(addressPoco).Id;
                            }

                            // This will ensure only one address is set as default address for the same type
                            var typeWiseAddresses = new List<Lms_CustomerAddressMappingPoco>();
                            if (customerAddress.IsDefault)
                            {
                                typeWiseAddresses = customerAddressList.Where(c => c.CustomerId == customerAddress.CustomerId && c.AddressTypeId == customerAddress.AddressTypeId).ToList();
                                if (typeWiseAddresses.Count > 0)
                                {
                                    foreach (var item in typeWiseAddresses)
                                    {
                                        item.IsDefault = false;
                                        _customerAddressMappingLogic.Update(item);
                                    }
                                }
                            }

                            var addressMappingList = _customerAddressMappingLogic.GetList();
                            if (customerAddress.AddressTypeId == 0)
                            {
                                Lms_CustomerAddressMappingPoco customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();

                                if (billingAddressMappingId != "")
                                {
                                    customerAddressMappingPoco = addressMappingList.Where(c => c.Id == Convert.ToInt32(billingAddressMappingId)).FirstOrDefault();
                                    customerAddressMappingPoco.AddressTypeId = (byte)Enum_AddressType.Billing;
                                    customerAddressMappingPoco.AddressId = addressId;
                                    customerAddressMappingPoco.IsDefault = true;
                                    _customerAddressMappingLogic.Update(customerAddressMappingPoco);
                                }
                                else {
                                    customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();
                                    customerAddressMappingPoco.CustomerId = customerAddress.CustomerId;
                                    customerAddressMappingPoco.AddressId = addressId;
                                    customerAddressMappingPoco.AddressTypeId = (byte)Enum_AddressType.Billing;
                                    customerAddressMappingPoco.IsDefault = true;
                                    _customerAddressMappingLogic.Add(customerAddressMappingPoco);
                                }

                                if (shippingAddressMappingId != "")
                                {
                                    customerAddressMappingPoco = addressMappingList.Where(c => c.Id == Convert.ToInt32(shippingAddressMappingId)).FirstOrDefault();
                                    customerAddressMappingPoco.AddressTypeId = (byte)Enum_AddressType.Shipping;
                                    customerAddressMappingPoco.AddressId = addressId;
                                    customerAddressMappingPoco.IsDefault = true;
                                    _customerAddressMappingLogic.Update(customerAddressMappingPoco);
                                }
                                else
                                {
                                    customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();
                                    customerAddressMappingPoco.CustomerId = customerAddress.CustomerId;
                                    customerAddressMappingPoco.AddressId = addressId;
                                    customerAddressMappingPoco.AddressTypeId = (byte)Enum_AddressType.Shipping;
                                    customerAddressMappingPoco.IsDefault = true;
                                    _customerAddressMappingLogic.Add(customerAddressMappingPoco);
                                }

                            }
                            else
                            {
                                Lms_CustomerAddressMappingPoco customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();

                                if (customerAddress.AddressTypeId == 1) {
                                    if (billingAddressMappingId != "")
                                    {
                                        customerAddressMappingPoco = addressMappingList.Where(c => c.Id == Convert.ToInt32(billingAddressMappingId)).FirstOrDefault();
                                        customerAddressMappingPoco.AddressId = addressId;
                                        customerAddressMappingPoco.IsDefault = true;
                                        _customerAddressMappingLogic.Update(customerAddressMappingPoco);
                                    }
                                    else
                                    {
                                        customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();
                                        customerAddressMappingPoco.CustomerId = customerAddress.CustomerId;
                                        customerAddressMappingPoco.AddressId = addressId;
                                        customerAddressMappingPoco.AddressTypeId = customerAddress.AddressTypeId;
                                        customerAddressMappingPoco.IsDefault = true;
                                        _customerAddressMappingLogic.Add(customerAddressMappingPoco);
                                    }
                                }
                                if (customerAddress.AddressTypeId == 2) {
                                    if (shippingAddressMappingId != "")
                                    {
                                        customerAddressMappingPoco = addressMappingList.Where(c => c.Id == Convert.ToInt32(shippingAddressMappingId)).FirstOrDefault();
                                        customerAddressMappingPoco.AddressId = addressId;
                                        customerAddressMappingPoco.IsDefault = true;
                                        _customerAddressMappingLogic.Update(customerAddressMappingPoco);
                                    }
                                    else {
                                        customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();
                                        customerAddressMappingPoco.CustomerId = customerAddress.CustomerId;
                                        customerAddressMappingPoco.AddressId = addressId;
                                        customerAddressMappingPoco.AddressTypeId = customerAddress.AddressTypeId;
                                        customerAddressMappingPoco.IsDefault = true;
                                        _customerAddressMappingLogic.Add(customerAddressMappingPoco);

                                    }
                                }
                            }

                        }

                        //scope.Complete();

                    }

                    //}
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
                var poco = _customerLogic.GetSingleById(Convert.ToInt32(id));
                if (poco != null)
                {
                    _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));
                    var accPoco = _chartOfAccountLogic.GetSingleById(poco.AccountId);
                    _chartOfAccountLogic.Remove(accPoco);

                    _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
                    var mappingAddresses = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == poco.Id).ToList();

                    if (mappingAddresses.Count > 0) {
                        foreach(var item in mappingAddresses)
                        {
                            _customerAddressMappingLogic.Remove(item);
                        }
                    }

                    _customerLogic.Remove(poco);
                }

                result = "Success";
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        [HttpPost]
        public IActionResult AddAddress([FromBody]dynamic addressData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (addressData != null)
                {
                    CustomerAddressMapping customerAddress = JsonConvert.DeserializeObject<CustomerAddressMapping>(JsonConvert.SerializeObject(addressData[0]));

                    //var jAddressObject = (JObject)addressData[0];
                    //var shippingAddressMappingId = Convert.ToString(jAddressObject.SelectToken("shippingAddressMappingId"));
                    //var billingAddressMappingId = Convert.ToString(jAddressObject.SelectToken("billingAddressMappingId"));

                    if (customerAddress != null)
                    {
                        _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                        var addressList = _addressLogic.GetList();

                        _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
                        var customerAddressList = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == customerAddress.CustomerId);

                        int newAddressId = 0;

                        //using (var scope = new TransactionScope())
                        //{
                            var existingAddress = addressList.Where(c => c.UnitNumber == customerAddress.UnitNumber && c.AddressLine == customerAddress.AddressLine && c.CityId == customerAddress.CityId).FirstOrDefault();
                            if (existingAddress != null)
                            {
                                existingAddress.ProvinceId = customerAddress.ProvinceId;
                                existingAddress.CountryId = customerAddress.CountryId;
                                existingAddress.PostCode = customerAddress.PostCode;
                                existingAddress.PrimaryPhoneNumber = customerAddress.PrimaryPhoneNumber;
                                existingAddress.Fax = customerAddress.Fax;
                                existingAddress.EmailAddress1 = customerAddress.EmailAddress1;
                                existingAddress.EmailAddress2 = customerAddress.EmailAddress1;
                                existingAddress.ContactPersonName = customerAddress.ContactPersonName;

                                newAddressId = _addressLogic.Update(existingAddress).Id;
                            }
                            else
                            {
                                Lms_AddressPoco addressPoco = new Lms_AddressPoco();
                                addressPoco.UnitNumber = customerAddress.UnitNumber;
                                addressPoco.AddressLine = customerAddress.AddressLine;
                                addressPoco.CityId = customerAddress.CityId;
                                addressPoco.ProvinceId = customerAddress.ProvinceId;
                                addressPoco.CountryId = customerAddress.CountryId;
                                addressPoco.PostCode = customerAddress.PostCode;
                                addressPoco.PrimaryPhoneNumber = customerAddress.PrimaryPhoneNumber;
                                addressPoco.Fax = customerAddress.Fax;
                                addressPoco.EmailAddress1 = customerAddress.EmailAddress1;
                                addressPoco.EmailAddress2 = customerAddress.EmailAddress1;
                                addressPoco.ContactPersonName = customerAddress.ContactPersonName;

                                newAddressId = _addressLogic.Add(addressPoco).Id;
                            }

                            // This will ensure only one address is set as default address for the same type
                            if (customerAddress.IsDefault == true)
                            {
                                var typeWiseAddresses = customerAddressList.Where(c => c.CustomerId == customerAddress.CustomerId && c.AddressTypeId == customerAddress.AddressTypeId).ToList();
                                if (typeWiseAddresses.Count > 0)
                                {
                                    foreach (var item in typeWiseAddresses)
                                    {
                                        item.IsDefault = false;
                                        _customerAddressMappingLogic.Update(item);
                                    }
                                }
                            }

                            var existingCustomerAddressMapping = customerAddressList.Where(c => c.AddressId == customerAddress.AddressId && c.AddressTypeId == customerAddress.AddressTypeId).FirstOrDefault();

                            if (existingCustomerAddressMapping != null)
                            {
                                existingCustomerAddressMapping.AddressId = newAddressId;
                                existingCustomerAddressMapping.IsDefault = customerAddress.IsDefault;
                                _customerAddressMappingLogic.Update(existingCustomerAddressMapping);
                            }
                            else
                            {
                                Lms_CustomerAddressMappingPoco customerAddressMappingPoco = new Lms_CustomerAddressMappingPoco();
                                customerAddressMappingPoco.CustomerId = customerAddress.CustomerId;
                                customerAddressMappingPoco.AddressId = customerAddress.AddressId;
                                customerAddressMappingPoco.AddressTypeId = customerAddress.AddressTypeId;
                                customerAddressMappingPoco.IsDefault = customerAddress.IsDefault;
                                _customerAddressMappingLogic.Add(customerAddressMappingPoco);
                            }

                            //scope.Complete();

                            result = newAddressId.ToString();

                        //}
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult RemoveAddress([FromBody]dynamic addressData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (addressData != null)
                {
                    var addressMappingData = (JObject)addressData[0];
                    var customerId = addressMappingData["customerId"].Value<int>();
                    var addressId = addressMappingData["addressId"].Value<int>();
                    var addressTypeId = addressMappingData["addressTypeId"].Value<int>();

                    _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
                    var existingEntry = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == customerId && c.AddressId == addressId && c.AddressTypeId == addressTypeId).FirstOrDefault();

                    if (existingEntry != null)
                    {
                        _customerAddressMappingLogic.Remove(existingEntry);
                        result = "Success";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        private List<Lms_CustomerPoco> GetCustomerData(int count)
        {
            var customerList = new List<Lms_CustomerPoco>();
            if (count > 0)
            {
                customerList = _customerLogic.GetList().OrderByDescending(c => c.AccountId).Take(count).ToList();
            }
            else if (count == 0)
            {
                customerList = _customerLogic.GetList().OrderByDescending(c => c.AccountId).ToList();
            }

            return customerList;
        }

        private List<ViewModel_CustomerAddress> GetCustomerAddressData(int id)
        {
            List<ViewModel_CustomerAddress> customerAddressViewModel = new List<ViewModel_CustomerAddress>();

            _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
            var customerAddresses = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == id).ToList();

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            var cities = _cityLogic.GetList();
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            var provinces = _provinceLogic.GetList();
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
            var countries = _countryLogic.GetList();
            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            var addresses = _addressLogic.GetList();

            foreach (var custAddress in customerAddresses)
            {
                ViewModel_CustomerAddress customerAddress = new ViewModel_CustomerAddress();

                var address = addresses.Where(c => c.Id == custAddress.AddressId).FirstOrDefault();
                customerAddress.CustomerId = custAddress.CustomerId;
                customerAddress.AddressId = custAddress.AddressId;
                customerAddress.AddressTypeId = custAddress.AddressTypeId;
                customerAddress.AddressTypeName = customerAddress.AddressTypeId == (byte)Enum_AddressType.Billing ? "Billing" : customerAddress.AddressTypeId == (byte)Enum_AddressType.Shipping ? "Shipping" : customerAddress.AddressTypeId == (byte)Enum_AddressType.Warehouse ? "Warehouse" : "";

                customerAddress.IsDefault = custAddress.IsDefault;

                customerAddress.UnitNumber = address.UnitNumber;
                customerAddress.HouseNumber = address.HouseNumber;
                customerAddress.AddressLine = address.AddressLine;
                customerAddress.CityId = address.CityId;
                customerAddress.CityName = cities.Where(c => c.Id == address.CityId).FirstOrDefault().CityName;
                customerAddress.ProvinceId = address.ProvinceId;
                customerAddress.ProvinceName = provinces.Where(c => c.Id == address.ProvinceId).FirstOrDefault().ShortCode;
                customerAddress.CountryId = address.CountryId;
                customerAddress.CountryName = countries.Where(c => c.Id == address.CountryId).FirstOrDefault().CountryName;
                customerAddress.PostCode = address.PostCode;

                customerAddress.MergedAddressLine = !string.IsNullOrEmpty(customerAddress.UnitNumber) ? customerAddress.UnitNumber + ", " : customerAddress.UnitNumber;
                customerAddress.MergedAddressLine += !string.IsNullOrEmpty(customerAddress.AddressLine) ? customerAddress.AddressLine + ", " : customerAddress.AddressLine;
                customerAddress.MergedAddressLine += customerAddress.CityName + ", " + customerAddress.ProvinceName + ", " + customerAddress.CountryName;
                customerAddress.MergedAddressLine += !string.IsNullOrEmpty(customerAddress.PostCode) ? ", " + customerAddress.PostCode : customerAddress.PostCode;

                customerAddress.Email = address.EmailAddress1;
                customerAddress.Phone = address.PrimaryPhoneNumber;
                customerAddress.Fax = address.Fax;
                customerAddress.ContactPerson = address.ContactPersonName;

                customerAddressViewModel.Add(customerAddress);
            }

            return customerAddressViewModel;
        }

        public JsonResult GetCustomers()
        {
            return Json(JsonConvert.SerializeObject(_customerLogic.GetList()));
        }

        public JsonResult GetCustomerById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var customer = _customerLogic.GetSingleById(Convert.ToInt32(id));

                if (customer != null)
                {
                    if (customer.FuelSurChargePercentage == null || customer.FuelSurChargePercentage <= 0)
                    {
                        _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                        var defaultFuelSurcharge = _configurationLogic.GetSingleById(1).DefaultFuelSurcharge;
                        customer.FuelSurChargePercentage = defaultFuelSurcharge;
                    }
                    return Json(JsonConvert.SerializeObject(customer));
                }

            }
            return Json(string.Empty);

        }

        public JsonResult GetCustomerDefaultShippingAddressById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));

                var customerDefaultShippingAddress = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == Convert.ToInt32(id) && c.AddressTypeId == (byte)Enum_AddressType.Shipping && c.IsDefault == true).FirstOrDefault();
                if (customerDefaultShippingAddress != null)
                {
                    return Json(JsonConvert.SerializeObject(customerDefaultShippingAddress));
                }
            }

            return Json(string.Empty);
        }

        public JsonResult GetCustomerDefaultBillingAddressById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));

                var customerDefaultBillingAddress = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == Convert.ToInt32(id) && c.AddressTypeId == (byte)Enum_AddressType.Billing && c.IsDefault == true).FirstOrDefault();
                if (customerDefaultBillingAddress != null)
                {
                    return Json(JsonConvert.SerializeObject(customerDefaultBillingAddress));
                }
            }

            return Json(string.Empty);
        }

        public JsonResult GetCustomerAddressesById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var customerAddresses = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == Convert.ToInt32(id));
                return Json(JsonConvert.SerializeObject(customerAddresses));
            }
            else
            {
                return Json(string.Empty);
            }
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