using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
    /// <summary>
    /// Some refactors needed in method names with their purposess
    /// </summary>
    public class InvoiceController : Controller
    {
        private Lms_InvoiceLogic _invoiceLogic;
        private Lms_InvoiceWayBillMappingLogic _invoiceWayBillMappingLogic;
        private Lms_OrderLogic _orderLogic;
        private Lms_OrderStatusLogic _orderStatusLogic;
        private Lms_CustomerLogic _customerLogic;

        private readonly LogisticsContext _dbContext;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public InvoiceController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _invoiceLogic = new Lms_InvoiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoicePoco>(_dbContext));
            _invoiceWayBillMappingLogic = new Lms_InvoiceWayBillMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoiceWayBillMappingPoco>(_dbContext));
            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();

            var customerList = _invoiceLogic.GetList();
            return View(GetPendingWaybillsForInvoice());
        }


        private List<PendingWaybillsForInvoice> GetPendingWaybillsForInvoice()
        {

            List<PendingWaybillsForInvoice> pendingInvoices;
            var combineOrdersForInvoice = new List<PendingWaybillsForInvoice>();

            try
            {
                var returnedResult = _invoiceLogic.GetPendingInvoiceOrders();
                var parsedObject = JObject.Parse(returnedResult);
                var jsonArrayString = parsedObject.SelectToken("ReturnedValue").ToString();
                var jsonArray = JArray.Parse(jsonArrayString);

                pendingInvoices = new List<PendingWaybillsForInvoice>();
                pendingInvoices = JsonConvert.DeserializeObject<List<PendingWaybillsForInvoice>>(JsonConvert.SerializeObject(jsonArray));


                foreach (var pendingInvoice in pendingInvoices)
                {
                    combineOrdersForInvoice = new List<PendingWaybillsForInvoice>();

                    var orders = pendingInvoices.Where(c => c.WayBillNumber == pendingInvoice.WayBillNumber).ToList();
                    if (orders.Count > 1)
                    {
                        var singleOrder = orders.Where(c => c.OrderTypeId == 1).FirstOrDefault();
                        var returnOrder = orders.Where(c => c.OrderTypeId == 2).FirstOrDefault();

                        var combineOrderForInvoice = new PendingWaybillsForInvoice();

                        combineOrderForInvoice.WayBillNumber = singleOrder.WayBillNumber;
                        combineOrderForInvoice.BillerName = singleOrder.BillerName;
                        combineOrderForInvoice.BillerDepartment = singleOrder.BillerDepartment;
                        combineOrderForInvoice.TotalOrderCost = singleOrder.TotalOrderCost;
                        combineOrderForInvoice.ReturnOrderCost = returnOrder.TotalOrderCost;

                        combineOrdersForInvoice.Add(combineOrderForInvoice);
                    }
                    else
                    {
                        combineOrdersForInvoice.Add(orders.FirstOrDefault());
                    }
                }
            }
            catch (Exception e)
            {

            }

            return combineOrdersForInvoice;
        }







        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewInvoicedData", GetInvoicedOrders());
        }

        [HttpGet]
        public IActionResult PartialPendingInvoiceDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewPendingData", GetPendingWaybillsForInvoice());
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic invoiceData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (invoiceData != null)
                {
                    var wayBillNumberList = JArray.Parse(JsonConvert.SerializeObject(invoiceData[0]));

                    var orders = _orderLogic.GetList();
                    var orderStatuses = _orderStatusLogic.GetList();

                    using (var scope = new TransactionScope())
                    {
                        foreach (var item in wayBillNumberList)
                        {
                            var wbNumber = item.SelectToken("wbillNumber").ToString();

                            orders = orders.Where(c => c.WayBillNumber == wbNumber).ToList();
                            foreach (var order in orders)
                            {
                                var status = orderStatuses.Where(c => c.OrderId == order.Id).FirstOrDefault();
                                _orderStatusLogic.Update(status);
                            }

                        }

                        scope.Complete();

                        result = "Success";

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Remove(string id)
        {
            var result = "";
            try
            {
                var orders = _orderLogic.GetList().Where(c => c.WayBillNumber == id).ToList();
                var dispatchedList = _orderStatusLogic.GetList();

                dispatchedList = (from dispatch in dispatchedList
                                  join order in orders on dispatch.OrderId equals order.Id
                                  select dispatch).ToList();

                using (var scope = new TransactionScope())
                {
                    foreach (var item in dispatchedList)
                    {
                        item.IsDispatched = null;
                        item.DispatchedDatetime = null;
                        item.DispatchedToEmployeeId = null;

                        _orderStatusLogic.Update(item);
                    }

                    scope.Complete();

                    result = "Success";
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }


        private List<InvoiceViewModel> GetInvoicedOrders()
        {
            List<InvoiceViewModel> invoicesViewModels = new List<InvoiceViewModel>();

            var invoiceList = _invoiceLogic.GetList().Where(c => c.PaidAmount == null).ToList();
            var invoiceWbMappingList = _invoiceWayBillMappingLogic.GetList();

            var invoiceListNew = (from order in invoiceList
                                  join mapping in invoiceWbMappingList on order.Id equals mapping.InvoiceId
                                  select order).ToList();






            return null;

        }












        private void ValidateSession()
        {
            if (HttpContext.Session.GetString("SessionData") != null)
            {
                sessionData = JsonConvert.DeserializeObject<SessionData>(HttpContext.Session.GetString("SessionData"));
                if (sessionData == null)
                {
                    Response.Redirect("Login/Index");
                }
            }
            else
            {
                Response.Redirect("Login/InvalidLocation");
            }
        }

    }
}