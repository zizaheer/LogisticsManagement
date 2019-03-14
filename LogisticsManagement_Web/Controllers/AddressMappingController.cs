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
    public class AddressMappingController : Controller
    {
        private Lms_AddressMappingLogic _addressMappingLogic;
        private readonly LogisticsContext _dbContext;

        public AddressMappingController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _addressMappingLogic = new Lms_AddressMappingLogic(new EntityFrameworkGenericRepository<Lms_AddressMappingPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _addressMappingLogic.GetAllList();
            return View();
        }
    }
}