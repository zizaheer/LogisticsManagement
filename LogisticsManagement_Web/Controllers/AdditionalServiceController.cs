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
    public class AdditionalServiceController : Controller
    {
        private Lms_AdditionalServiceLogic _additionalServiceLogic;
        private readonly LogisticsContext _dbContext;

        public AdditionalServiceController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _additionalServiceLogic = new Lms_AdditionalServiceLogic(new EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _additionalServiceLogic.GetAllList();
            return View();
        }
    }
}