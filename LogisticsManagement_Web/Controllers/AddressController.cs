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
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public AddressController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var addressList = _addressLogic.GetList();
            return View();
        }


        public JsonResult GetAddresses()
        {
            var addressList = _addressLogic.GetList();
            return Json(JsonConvert.SerializeObject(addressList));
        }

        public JsonResult GetAddressesForAutoComplete()
        {
            var addressList = _addressLogic.GetList();

            List<AddressViewModelForAutoComplete> addressesForAutoComplete = new List<AddressViewModelForAutoComplete>();
            if (addressList.Count > 0)
            {
                foreach (var address in addressList)
                {
                    AddressViewModelForAutoComplete addressForAutoComplete = new AddressViewModelForAutoComplete();
                    addressForAutoComplete.label = (address.UnitNumber == null ? "" : address.UnitNumber + ", ") + address.AddressLine + "  (" + address.Id + ")";
                    addressForAutoComplete.value = address.Id.ToString();

                    addressForAutoComplete.Id = address.Id.ToString();
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

        public JsonResult GetAddressLines()
        {
            var addressList = _addressLogic.GetList().Select(c => c.AddressLine);
            return Json(JsonConvert.SerializeObject(addressList));
        }

        public JsonResult GetAddressById(string id)
        {
            var address = _addressLogic.GetList().Where(c => c.Id == Convert.ToInt32(id));
            return Json(JsonConvert.SerializeObject(address));
        }

        public JsonResult GetAddressByAddressLine(string addressline)
        {
            var address = _addressLogic.GetList().Where(c => c.AddressLine == (addressline.Trim()));
            return Json(JsonConvert.SerializeObject(address));
        }

    }

}