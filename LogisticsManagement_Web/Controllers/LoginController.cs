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
using Newtonsoft.Json;

namespace LogisticsManagement_Web.Controllers
{
    public class LoginController : Controller
    {
        private App_UserLogic _userLogic;
        private readonly LogisticsContext _dbContext;
        public LoginController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _userLogic = new App_UserLogic(new EntityFrameworkGenericRepository<App_UserPoco>(_dbContext));
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string userName, string userPassword)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userPassword))
            {
                if (_userLogic.IsCredentialsValid(userName, userPassword, out App_UserPoco outUserData))
                {
                    SessionData sessionData = new SessionData();
                    sessionData.UserId = outUserData.Id;
                    sessionData.GroupId = outUserData.GroupId;
                    sessionData.BranchId = outUserData.BranchId;
                    sessionData.UserName = outUserData.UserName;
                    sessionData.FirstName = outUserData.FirstName;
                    sessionData.MiddleName = outUserData.MiddleName;
                    sessionData.LastName = outUserData.LastName;
                    sessionData.EmailAddress = outUserData.EmailAddress;
                    sessionData.Address = outUserData.Address;
                    sessionData.CityId = outUserData.CityId;
                    sessionData.ProvinceId = outUserData.ProvinceId;
                    sessionData.CountryId = outUserData.CountryId;
                    sessionData.PostCode = outUserData.PostCode;
                    sessionData.PhoneNumber = outUserData.PhoneNumber;
                    //sessionData.ProfilePicture = outUserData.ProfilePicture;
                    sessionData.LoggedInEmployeeId = outUserData.EmployeeId;

                    HttpContext.Session.SetString("SessionData", JsonConvert.SerializeObject(sessionData));
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("SessionData");
            return RedirectToAction("Index");
        }

        public IActionResult InvalidLocation()
        {
            HttpContext.Session.Remove("SessionData");
            return View();
        }

    }
}