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
    public class ProvinceController : Controller
    {
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;

        private readonly LogisticsContext _dbContext;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public ProvinceController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var provinceList = _provinceLogic.GetList();
            var countryList = _countryLogic.GetList();
            ViewModel_Province viewModel_Prov = new ViewModel_Province();
            viewModel_Prov.Provinces = provinceList;
            viewModel_Prov.Countries = countryList;
            return View(viewModel_Prov);
        }

        [HttpGet]
        public JsonResult GetAllProvinces()
        {
            string result = "";
            try
            {
                var provinceList = _provinceLogic.GetList();
                if (provinceList != null && provinceList.Count > 0)
                {
                    result = JsonConvert.SerializeObject(provinceList);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic provData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (provData != null)
                {
                    App_ProvincePoco provPoco = JsonConvert.DeserializeObject<App_ProvincePoco>(JsonConvert.SerializeObject(provData[0]));

                    using (var scope = new TransactionScope())
                    {
                        provPoco.IsActive = true;
                        provPoco.CreateDate = DateTime.Now;
                        provPoco.CreatedBy = sessionData.UserId;

                        var provId = _provinceLogic.Add(provPoco).Id;
                        if (provId > 0)
                        {
                            scope.Complete();
                            result = provId.ToString();
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
        public IActionResult Update([FromBody]dynamic provData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (provData != null)
                {
                    App_ProvincePoco provPoco = JsonConvert.DeserializeObject<App_ProvincePoco>(JsonConvert.SerializeObject(provData[0]));

                    using (var scope = new TransactionScope())
                    {
                        var existingProvince = _provinceLogic.GetSingleById(provPoco.Id);

                        existingProvince.ProvinceName = provPoco.ProvinceName;
                        existingProvince.ShortCode = provPoco.ShortCode;
                        existingProvince.CountryId = provPoco.CountryId;
                        //existingProvince.IsDefault = provPoco.IsDefault;

                        var provId = _provinceLogic.Update(existingProvince).Id;
                        if (provId > 0)
                        {
                            scope.Complete();
                            result = provId.ToString();
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
                var poco = _provinceLogic.GetSingleById(Convert.ToInt32(id));
                if (poco != null)
                {
                    _provinceLogic.Remove(poco);
                    result = "Success";
                }
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetProvinceById(string Id)
        {
            string result = "";
            try
            {
                var provInfo = _provinceLogic.GetSingleById(Convert.ToInt32(Id));
                if (provInfo != null)
                {
                    result = JsonConvert.SerializeObject(provInfo);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetProvincesByCountry(string Id)
        {
            string result = "";
            try
            {
                var provinceList = _provinceLogic.GetList().Where(c => c.CountryId == Convert.ToInt32(Id)).ToList();
                if (provinceList != null && provinceList.Count > 0)
                {
                    result = JsonConvert.SerializeObject(provinceList);
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