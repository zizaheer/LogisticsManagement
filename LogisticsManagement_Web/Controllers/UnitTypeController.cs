﻿using System;
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
    public class UnitTypeController : Controller
    {
        private Lms_UnitTypeLogic _unitTypeLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public UnitTypeController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _unitTypeLogic.GetList();
            return View();
        }

        [HttpGet]
        public JsonResult GetAllTypes()
        {
            string result = "";
            try
            {
                var typeList = _unitTypeLogic.GetList();
                if (typeList != null && typeList.Count > 0)
                {
                    result = JsonConvert.SerializeObject(typeList);
                }

            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }
    }
}