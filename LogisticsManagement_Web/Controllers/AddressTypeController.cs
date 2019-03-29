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
    public class AddressTypeController : Controller
    {
        private Lms_AddressTypeLogic _addressTypeLogic;
        private readonly LogisticsContext _dbContext;

        public AddressTypeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _addressTypeLogic = new Lms_AddressTypeLogic(new EntityFrameworkGenericRepository<Lms_AddressTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _addressTypeLogic.GetList();
            return View();
        }
    }
}