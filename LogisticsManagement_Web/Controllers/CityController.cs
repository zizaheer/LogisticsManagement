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
    public class CityController : Controller
    {
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private readonly LogisticsContext _dbContext;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public CityController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _cityLogic = new App_CityLogic(cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var cityList = _cityLogic.GetList();
            var provinceList = _provinceLogic.GetList();
            var countryList = _countryLogic.GetList();
            ViewModel_City viewModel_City = new ViewModel_City();
            viewModel_City.Cities = cityList;
            viewModel_City.Provinces = provinceList;
            viewModel_City.Countries = countryList;
            return View(viewModel_City);
        }

        [HttpGet]
        public JsonResult GetAllCities()
        {
            string result = "";
            try
            {
                var cityList = _cityLogic.GetList();
                
                if (cityList != null && cityList.Count > 0)
                {
                    result = JsonConvert.SerializeObject(cityList);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic cityData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (cityData != null)
                {
                    App_CityPoco cityPoco = JsonConvert.DeserializeObject<App_CityPoco>(JsonConvert.SerializeObject(cityData[0]));

                    using (var scope = new TransactionScope())
                    {
                        cityPoco.IsActive = true;
                        cityPoco.CreateDate = DateTime.Now;
                        cityPoco.CreatedBy = sessionData.UserId;

                        var cityId = _cityLogic.Add(cityPoco).Id;
                        if (cityId > 0)
                        {
                            scope.Complete();
                            result = cityId.ToString();
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
        public IActionResult Update([FromBody]dynamic cityData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (cityData != null)
                {
                    App_CityPoco cityPoco = JsonConvert.DeserializeObject<App_CityPoco>(JsonConvert.SerializeObject(cityData[0]));

                    using (var scope = new TransactionScope())
                    {
                        var existingCity = _cityLogic.GetSingleById(cityPoco.Id);
                        existingCity.CityName = cityPoco.CityName;
                        existingCity.ProvinceId = cityPoco.ProvinceId;
                        existingCity.CountryId = cityPoco.CountryId;

                        var cityId = _cityLogic.Update(existingCity).Id;
                        if (cityId > 0)
                        {
                            scope.Complete();
                            result = cityId.ToString();
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
                var poco = _cityLogic.GetSingleById(Convert.ToInt32(id));
                if (poco != null)
                {
                    _cityLogic.Remove(poco);
                    result = "Success";
                }
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetCitiesByProvince(string Id)
        {
            string result = "";
            try
            {
                var cityList = _cityLogic.GetList().Where(c => c.ProvinceId == Convert.ToInt32(Id)).ToList();
                if (cityList != null && cityList.Count > 0)
                {
                    result = JsonConvert.SerializeObject(cityList);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetCityById(string Id)
        {
            string result = "";
            try
            {
                var cityInfo = _cityLogic.GetSingleById(Convert.ToInt32(Id));
                if (cityInfo != null)
                {
                    result = JsonConvert.SerializeObject(cityInfo);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetCitiesByCountry(string Id)
        {
            string result = "";
            try
            {
                var cityList = _cityLogic.GetList().Where(c => c.CountryId == Convert.ToInt32(Id)).ToList();
                if (cityList != null && cityList.Count > 0)
                {
                    result = JsonConvert.SerializeObject(cityList);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
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