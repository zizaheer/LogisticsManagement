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
    public class WeightScaleController : Controller
    {
        private Lms_WeightScaleLogic _weightScaleLogic;
        private readonly LogisticsContext _dbContext;

        public WeightScaleController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _weightScaleLogic = new Lms_WeightScaleLogic(new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _weightScaleLogic.GetList();
            return View();
        }
    }
}