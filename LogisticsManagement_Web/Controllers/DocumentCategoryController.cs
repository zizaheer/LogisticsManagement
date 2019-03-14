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
    public class DocumentCategoryController : Controller
    {
        private App_DocumentCategoryLogic _documentCategoryLogic;
        private readonly LogisticsContext _dbContext;

        public DocumentCategoryController(LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _documentCategoryLogic = new App_DocumentCategoryLogic(new EntityFrameworkGenericRepository<App_DocumentCategoryPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _documentCategoryLogic.GetAllList();
            return View();
        }
    }
}