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
    public class AddressController : Controller
    {
        private Lms_AddressLogic _addressLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public AddressController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            return View(GetAddresses());
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic addressData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (addressData != null)
                {
                    Lms_AddressPoco addressPoco = JsonConvert.DeserializeObject<Lms_AddressPoco>(JsonConvert.SerializeObject(addressData));

                    if (addressPoco.Id < 1 && addressPoco.AddressLine.Trim() != string.Empty)
                    {
                        addressPoco.CreatedBy = sessionData.UserId;
                        var address = _addressLogic.Add(addressPoco);

                        result = "Success";
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Update([FromBody]dynamic addressData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (addressData != null)
                {
                    Lms_AddressPoco addressPoco = JsonConvert.DeserializeObject<Lms_AddressPoco>(JsonConvert.SerializeObject(addressData));

                    if (addressPoco.Id > 0 && addressPoco.AddressLine.Trim() != string.Empty)
                    {
                        var existingAddress = _addressLogic.GetSingleById(addressPoco.Id);
                        // it is required to pull existing data first, 
                        // cause there are some data which do not come from UI

                        existingAddress.UnitNumber = addressPoco.UnitNumber;
                        existingAddress.AddressLine = addressPoco.AddressLine;
                        existingAddress.CityId = addressPoco.CityId;
                        existingAddress.ProvinceId = addressPoco.ProvinceId;
                        existingAddress.CountryId = addressPoco.CountryId;
                        existingAddress.PostCode = addressPoco.PostCode;
                        existingAddress.EmailAddress1 = addressPoco.EmailAddress1;
                        existingAddress.EmailAddress2 = addressPoco.EmailAddress2;
                        existingAddress.MobileNumber = addressPoco.MobileNumber;
                        existingAddress.Fax = addressPoco.Fax;
                        existingAddress.PrimaryPhoneNumber = addressPoco.PrimaryPhoneNumber;
                        existingAddress.ContactPersonName = addressPoco.ContactPersonName;


                        var poco = _addressLogic.Update(existingAddress);
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
                var poco = _addressLogic.GetSingleById(Convert.ToInt32(id));
                if (poco != null)
                {
                    _addressLogic.Remove(poco);
                    result = "Success";
                }
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            List<ViewModel_AddressDetail> filteredAddresses = new List<ViewModel_AddressDetail>();

            try
            {



                var addresses = _addressLogic.GetList();
                foreach (var address in addresses)
                {

                    ViewModel_AddressDetail filteredAddress = new ViewModel_AddressDetail();
                    filteredAddress.AddressId = address.Id;
                    filteredAddress.UnitNumber = address.UnitNumber;
                    filteredAddress.HouseNumber = address.HouseNumber;
                    filteredAddress.StreetNumber = address.StreetNumber;
                    filteredAddress.AddressLine = address.AddressLine;
                    filteredAddress.CityName = _cityLogic.GetSingleById(address.CityId).CityName;
                    filteredAddress.ProvinceShortCode = _provinceLogic.GetSingleById(address.ProvinceId).ShortCode;
                    filteredAddress.CountryName = _countryLogic.GetSingleById(address.CountryId).CountryName;
                    filteredAddress.PostCode = address.PostCode;
                    filteredAddress.ContactPersonName = address.ContactPersonName;
                    filteredAddress.Fax = address.Fax;
                    filteredAddress.PrimaryPhoneNumber = address.PrimaryPhoneNumber;
                    filteredAddress.PhoneNumber2 = address.PhoneNumber2;
                    filteredAddress.MobileNumber = address.MobileNumber;
                    filteredAddress.EmailAddress1 = address.EmailAddress1;

                    filteredAddresses.Add(filteredAddress);
                }

            }
            catch (Exception ex)
            {

            }

            return PartialView("_PartialViewAddressData", filteredAddresses);
        }

        public JsonResult GetAddressesForAutoComplete()
        {
            var addressList = _addressLogic.GetList();

            List<ViewModel_AddressDetail> addressesForAutoComplete = new List<ViewModel_AddressDetail>();
            if (addressList.Count > 0)
            {
                foreach (var address in addressList)
                {
                    ViewModel_AddressDetail addressForAutoComplete = new ViewModel_AddressDetail();
                    addressForAutoComplete.label = (address.UnitNumber == null ? "" : address.UnitNumber + ", ") + address.AddressLine + "  (" + address.Id + ")";
                    addressForAutoComplete.value = address.Id.ToString();

                    addressForAutoComplete.AddressId = address.Id;
                    addressForAutoComplete.UnitNumber = address.UnitNumber;

                    addressForAutoComplete.HouseNumber = address.HouseNumber;
                    addressForAutoComplete.StreetNumber = address.StreetNumber;
                    addressForAutoComplete.AddressLine = address.AddressLine;
                    addressForAutoComplete.CityId = address.CityId.ToString();
                    addressForAutoComplete.ProvinceId = address.ProvinceId.ToString();
                    addressForAutoComplete.CountryId = address.CountryId.ToString();
                    addressForAutoComplete.PostCode = address.PostCode;

                    addressesForAutoComplete.Add(addressForAutoComplete);
                }
            }

            return Json(JsonConvert.SerializeObject(addressesForAutoComplete));
        }

        private ViewModel_Address GetAddresses()
        {
            ViewModel_Address addressViewModels = new ViewModel_Address();
            addressViewModels.Addresses = _addressLogic.GetList();
            addressViewModels.Cities = _cityLogic.GetList();
            addressViewModels.Provinces = _provinceLogic.GetList();
            addressViewModels.Countries = _countryLogic.GetList();

            return addressViewModels;
        }

        public JsonResult GetAddressLines()
        {
            var addressList = _addressLogic.GetList().Select(c => c.AddressLine);
            return Json(JsonConvert.SerializeObject(addressList));
        }

        public JsonResult GetAddressById(string id)
        {
            var address = _addressLogic.GetList().Where(c => c.Id == Convert.ToInt32(id)).FirstOrDefault();
            return Json(JsonConvert.SerializeObject(address));
        }

        public JsonResult GetAddressByAddressLine(string addressline)
        {
            var address = _addressLogic.GetList().Where(c => c.AddressLine == (addressline.Trim()));
            return Json(JsonConvert.SerializeObject(address));
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