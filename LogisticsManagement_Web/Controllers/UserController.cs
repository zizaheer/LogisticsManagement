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
    public class UserController : Controller
    {
        private App_UserLogic _userLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private App_UserGroupLogic _userGroupLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public UserController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _userLogic = new App_UserLogic(new EntityFrameworkGenericRepository<App_UserPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));

            ViewBag.UserGroups = Enum.GetValues(typeof(Enum_UserGroup)).Cast<Enum_UserGroup>();
            ViewBag.Cities = _cityLogic.GetList().Select(c => new App_CityPoco { Id = c.Id, CityName = c.CityName }).ToList();
            ViewBag.Provinces = _provinceLogic.GetList().Select(c => new App_ProvincePoco { Id = c.Id, ShortCode = c.ShortCode, IsDefault = c.IsDefault }).ToList();
            ViewBag.Countries = _countryLogic.GetList().Select(c => new App_CountryPoco { Id = c.Id, CountryName = c.CountryName, IsDefault = c.IsDefault }).ToList();

            return View(GetUserData());

        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewUserData", GetUserData());
        }

        private List<App_UserPoco> GetUserData()
        {
            var filteredUsers = _userLogic.GetList().Select(c => new App_UserPoco
            {
                Id = c.Id,
                UserName = c.UserName,
                FirstName = c.FirstName,
                LastName = c.LastName,
                EmailAddress = c.EmailAddress,
                BranchId = c.BranchId,
                Address = c.Address,
                PostCode = c.PostCode,
                CityId = c.CityId,
                ProvinceId = c.ProvinceId,
                CountryId = c.CountryId,
                GroupId = c.GroupId,
                PhoneNumber = c.PhoneNumber,
                ProfilePicture = c.ProfilePicture,
                IsInitialPasswordChanged = c.IsInitialPasswordChanged,
                IsActive = c.IsActive
            }).ToList();

            return filteredUsers;
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic userData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (userData != null)
                {
                    App_UserPoco userPoco = JsonConvert.DeserializeObject<App_UserPoco>(JsonConvert.SerializeObject(userData[0]));
                    var profileImageInfo = Convert.ToString(userData[1]);

                    if (userPoco.Id < 1 && userPoco.UserName.Trim() != string.Empty && userPoco.Password != string.Empty)
                    {
                        if (profileImageInfo != null && profileImageInfo != "" && profileImageInfo.Contains(","))
                        {
                            var base64String = profileImageInfo.Split(",")[1];
                            if (!string.IsNullOrEmpty(base64String))
                            {
                                userPoco.ProfilePicture = Convert.FromBase64String(base64String);
                            }

                        }

                        userPoco.CreatedBy = sessionData.UserId;
                        result = _userLogic.Add(userPoco).Id.ToString();

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Update([FromBody]dynamic userData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (userData != null)
                {
                    App_UserPoco userPoco = JsonConvert.DeserializeObject<App_UserPoco>(JsonConvert.SerializeObject(userData[0]));
                    var profileImageInfo = Convert.ToString(userData[1]);

                    if (userPoco.Id > 0 && userPoco.UserName.Trim() != string.Empty)
                    {
                        var user = _userLogic.GetSingleById(userPoco.Id);

                        user.FirstName = userPoco.FirstName;
                        user.LastName = userPoco.LastName;
                        user.GroupId = userPoco.GroupId;
                        user.BranchId = userPoco.BranchId;
                        user.EmailAddress = userPoco.EmailAddress;
                        user.Address = userPoco.Address;
                        user.CityId = userPoco.CityId;
                        user.ProvinceId = userPoco.ProvinceId;
                        user.CountryId = userPoco.CountryId;
                        user.PhoneNumber = userPoco.PhoneNumber;

                        if (profileImageInfo != null && profileImageInfo != "")
                        {
                            var base64String = profileImageInfo.Split(",")[1];
                            if (!string.IsNullOrEmpty(base64String))
                            {
                                user.ProfilePicture = Convert.FromBase64String(base64String);
                            }

                        }

                        user.IsInitialPasswordChanged = userPoco.IsInitialPasswordChanged;
                        user.IsActive = userPoco.IsActive;

                        var poco = _userLogic.Update(user);
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
        public IActionResult UpdatePassword([FromBody]dynamic userData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (userData != null)
                {
                    App_UserPoco userPoco = JsonConvert.DeserializeObject<App_UserPoco>(JsonConvert.SerializeObject(userData));

                    if (userPoco.Id > 0 && userPoco.UserName.Trim() != string.Empty)
                    {
                        var user = _userLogic.GetSingleById(userPoco.Id);
                        user.Password = userPoco.Password;

                        var poco = _userLogic.UpdatePassword(user);
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
                var poco = _userLogic.GetSingleById(Convert.ToInt32(id));
                _userLogic.Remove(poco);

                result = "Success";
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        public JsonResult GetUserById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = _userLogic.GetSingleById(Convert.ToInt32(id));
                return Json(JsonConvert.SerializeObject(user));
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