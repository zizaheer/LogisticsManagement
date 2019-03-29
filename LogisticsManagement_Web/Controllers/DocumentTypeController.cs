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
    public class DocumentTypeController : Controller
    {
        private App_DocumentTypeLogic _documentTypeLogic;
        private readonly LogisticsContext _dbContext;

        public DocumentTypeController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _documentTypeLogic = new App_DocumentTypeLogic(new EntityFrameworkGenericRepository<App_DocumentTypePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _documentTypeLogic.GetList();
            return View();
        }
    }
}