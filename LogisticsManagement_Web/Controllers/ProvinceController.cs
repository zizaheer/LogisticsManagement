using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using Microsoft.AspNetCore.Mvc;


namespace LogisticsManagement_Web.Controllers
{
    public class ProvinceController : Controller
    {
        private App_ProvinceLogic _provinceLogic;
        private readonly LogisticsContext _dbContext;

        public ProvinceController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _provinceLogic = new App_ProvinceLogic(new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _provinceLogic.GetList();
            return View();
        }
    }
}