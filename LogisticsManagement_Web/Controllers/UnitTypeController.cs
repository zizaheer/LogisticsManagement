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
    public class UnitTypeController : Controller
    {
        private Lms_UnitTypeLogic _unitTypeLogic;
        private readonly LogisticsContext _dbContext;

        public UnitTypeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _unitTypeLogic = new Lms_UnitTypeLogic(new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _unitTypeLogic.GetAllList();
            return View();
        }
    }
}