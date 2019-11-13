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
        SessionData sessionData = new SessionData();
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
            return PartialView("_PartialViewCompanyData", _companyInfoLogic.GetList());
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
                        var companyInfo = _companyInfoLogic.Add(companyPoco);
                        result = companyInfo.Id.ToString();

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

                            var companyInfo = _companyInfoLogic.Update(existingCompany);
                            result = companyInfo.Id.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

       

        public IActionResult  GetCompanyDataById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var companyInfo = _companyInfoLogic.GetSingleById(Convert.ToInt16(id));
                if (companyInfo != null)
                {

                    return Json(JsonConvert.SerializeObject(companyInfo));
                }
            }
            return Json(string.Empty);
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