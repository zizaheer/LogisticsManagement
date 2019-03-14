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
    public class AddressController : Controller
    {
        private Lms_AddressLogic _addressLogic;
        private readonly LogisticsContext _dbContext;

        public AddressController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _addressLogic = new Lms_AddressLogic(new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _addressLogic.GetAllList();
            return View();
        }
    }
}