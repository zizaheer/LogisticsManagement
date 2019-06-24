using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using Microsoft.AspNetCore.Mvc;
using LogisticsManagement_Web.Models;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace LogisticsManagement_Web.Controllers
{
    public class HomeController : Controller
    {
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public HomeController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {
            var currentData = HttpContext.Session.GetString("SessionData");
            if (currentData != null) {
                sessionData = JsonConvert.DeserializeObject<SessionData>(currentData);
                if (sessionData == null)
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            
            
            sessionData = JsonConvert.DeserializeObject<SessionData>(HttpContext.Session.GetString("SessionData"));
            if (sessionData == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.UserId = sessionData.UserId;
            return View();
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
