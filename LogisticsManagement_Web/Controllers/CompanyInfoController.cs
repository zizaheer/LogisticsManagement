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
    public class CompanyInfoController : Controller
    {
        private Lms_CompanyInfoLogic _companyInfoLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public CompanyInfoController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _companyInfoLogic = new Lms_CompanyInfoLogic(_cache, new EntityFrameworkGenericRepository<Lms_CompanyInfoPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var companyList = _companyInfoLogic.GetList();
            return View(companyList);
        }

        [HttpGet]
        public IActionResult LoadCompanyData(string id)
        {
            ValidateSession();
            return PartialView("_PartialViewCompanyData", GetCustomerData(Convert.ToInt32(id)));
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic companyData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (companyData != null)
                {
                    Lms_CompanyInfoPoco companyPoco = JsonConvert.DeserializeObject<Lms_CompanyInfoPoco>(JsonConvert.SerializeObject(companyData[0]));

                    if (!string.IsNullOrEmpty(companyPoco.CompanyName) && companyPoco.Id == 0)
                    {
                        _companyInfoLogic.Add(companyPoco);
                        result = "success";

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Update([FromBody]dynamic companyData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (companyData != null)
                {
                    Lms_CompanyInfoPoco companyPoco = JsonConvert.DeserializeObject<Lms_CompanyInfoPoco>(JsonConvert.SerializeObject(companyData[0]));

                    if (!string.IsNullOrEmpty(companyPoco.CompanyName) && companyPoco.Id > 0)
                    {
                        var existingCompany = _companyInfoLogic.GetSingleById(companyPoco.Id);
                        if (existingCompany != null)
                        {
                            existingCompany.CompanyName = companyPoco.CompanyName;
                            //existingCompany.MainAddress = companyPoco.MainAddress;
                            //existingCompany.CityId = companyPoco.CityId;
                            existingCompany.CompanyLogo = companyPoco.CompanyLogo;
                            //existingCompany.CompanyRegistrationNo = companyPoco.CompanyRegistrationNo;
                            //existingCompany.ContactNumber = companyPoco.ContactNumber;
                            //existingCompany.ContactPerson = companyPoco.ContactPerson;
                            //existingCompany.CountryId = companyPoco.CountryId;
                            //existingCompany.EmailAddress = companyPoco.EmailAddress;
                            //existingCompany.Fax = companyPoco.Fax;
                            //existingCompany.PostCode = companyPoco.PostCode;
                            //existingCompany.ProvinceId = companyPoco.ProvinceId;
                            //existingCompany.TaxNumber = companyPoco.TaxNumber;
                            //existingCompany.Telephone = companyPoco.Telephone;

                            _companyInfoLogic.Update(existingCompany);
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
                var poco = _customerLogic.GetSingleById(Convert.ToInt32(id));
                if (poco != null)
                {
                    _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));
                    var accPoco = _chartOfAccountLogic.GetSingleById(poco.AccountId);
                    _chartOfAccountLogic.Remove(accPoco);

                    _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
                    var mappingAddresses = _customerAddressMappingLogic.GetList().Where(c => c.CustomerId == poco.Id).ToList();

                    if (mappingAddresses.Count > 0)
                    {
                        foreach (var item in mappingAddresses)
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