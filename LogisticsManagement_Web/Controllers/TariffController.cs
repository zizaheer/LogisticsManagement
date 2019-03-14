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
    public class TariffController : Controller
    {
        private Lms_TariffLogic _tariffLogic;
        private readonly LogisticsContext _dbContext;

        public TariffController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _tariffLogic = new Lms_TariffLogic(new EntityFrameworkGenericRepository<Lms_TariffPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _tariffLogic.GetAllList();
            return View();
        }
    }
}