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
    public class OrderTypeController : Controller
    {
        private Lms_OrderTypeLogic _orderTypeLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public OrderTypeController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _orderTypeLogic = new Lms_OrderTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _orderTypeLogic.GetList();
            return View();
        }
    }
}