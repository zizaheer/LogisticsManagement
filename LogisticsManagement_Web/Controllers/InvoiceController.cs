using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogisticsManagement_Web.Controllers
{
    public class InvoiceController : Controller
    {
        private Lms_InvoiceLogic _invoiceLogic;
        private Lms_OrderLogic _orderLogic;
        private Lms_OrderStatusLogic _orderStatusLogic;
        private Lms_CustomerLogic _customerLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public InvoiceController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _invoiceLogic = new Lms_InvoiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoicePoco>(_dbContext));
            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _invoiceLogic.GetList();
            return View();
        }


        private List<PendingWaybillsForInvoice> GetDeliveredOrders()
        {

            var orderList = _orderLogic.GetList().Where(c => c.OrderTypeId == 1 || c.OrderTypeId == 2).ToList();
            var dispatchedList = _orderStatusLogic.GetList();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));

            var customerList = _customerLogic.GetList();

            orderList = (from order in orderList
                         join dispatch in dispatchedList on order.Id equals dispatch.OrderId
                         where dispatch.IsDelivered == true
                         select order).ToList();

            List<PendingWaybillsForInvoice> _pendingList = new List<PendingWaybillsForInvoice>();

            foreach (var item in orderList)
            {
                PendingWaybillsForInvoice pendingWaybillsForInvoice = new PendingWaybillsForInvoice();
                pendingWaybillsForInvoice.WaybillNumber = item.WayBillNumber;
                pendingWaybillsForInvoice.BillerCustomerId = item.BillToCustomerId;
                pendingWaybillsForInvoice.BillerDepartment = item.DepartmentName;
                pendingWaybillsForInvoice.BillerName = customerList.Where(c => c.Id == item.BillToCustomerId).FirstOrDefault().CustomerName;
                pendingWaybillsForInvoice.BillerEmail = item.WayBillNumber;



            }



            return null; // orderList;

        }

    }
}