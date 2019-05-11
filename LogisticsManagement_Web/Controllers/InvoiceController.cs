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
                    //combineOrdersForInvoice = new List<PendingWaybillsForInvoice>();

                    var orders = pendingInvoices.Where(c => c.WayBillNumber == pendingInvoice.WayBillNumber).ToList();
                    if (orders.Count > 1)
                    {
                        var singleOrder = orders.Where(c => c.OrderTypeId == 1).FirstOrDefault();
                        var returnOrder = orders.Where(c => c.OrderTypeId == 2).FirstOrDefault();

                        var combineOrderForInvoice = new PendingWaybillsForInvoice();

                        combineOrderForInvoice.BillToCustomerId = singleOrder.BillToCustomerId;
                        combineOrderForInvoice.WayBillNumber = singleOrder.WayBillNumber;
                        combineOrderForInvoice.BillerName = singleOrder.BillerName;
                        combineOrderForInvoice.BillerDepartment = singleOrder.BillerDepartment;
                        combineOrderForInvoice.TotalOrderCost = singleOrder.TotalOrderCost;
                        combineOrderForInvoice.ReturnOrderCost = returnOrder.TotalOrderCost;

                        var existCount = combineOrdersForInvoice.Where(c => c.WayBillNumber == combineOrderForInvoice.WayBillNumber).ToList();
                        if (existCount.Count < 1)
                        {
                            combineOrdersForInvoice.Add(combineOrderForInvoice);
                        }
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

                    var countArray = ((JArray)wayBillNumberList).Count;
                    string[] wbNumbers = new string[countArray];

                    var orders = _orderLogic.GetList();
                    List<InvoiceViewModel> invoiceViewModels = new List<InvoiceViewModel>();

                    for (int i = 0; i < countArray; i++)
                    {
                        wbNumbers[i] = wayBillNumberList[i].SelectToken("wbillNumber").ToString();
                        InvoiceViewModel invoiceViewModel = new InvoiceViewModel();

                        invoiceViewModel.WayBillNumbers = wbNumbers[i];
                        invoiceViewModels.Add(invoiceViewModel);
                    }

                    orders = (from order in orders
                              join waybill in invoiceViewModels on order.WayBillNumber equals waybill.WayBillNumbers
                              select order).ToList();

                    using (var scope = new TransactionScope())
                    {
                        foreach (var item in orders)
                        {
                            var customerWiseOrders = orders.Where(c => c.BillToCustomerId == item.BillToCustomerId).ToList();
                            
                            int billerCustomerId;
                            string billerDepartment;
                            int createdBy = sessionData.UserId;

                            string[] customerWiseWbNumbers;
                            customerWiseWbNumbers = customerWiseOrders.Select(c => c.WayBillNumber).ToArray();
                            billerCustomerId = customerWiseOrders.FirstOrDefault().BillToCustomerId;
                            billerDepartment = customerWiseOrders.FirstOrDefault().DepartmentName;

                            _invoiceLogic.GenerateInvoice(billerCustomerId, billerDepartment, createdBy, customerWiseWbNumbers);

                            orders.RemoveAll(c => c.BillToCustomerId == item.BillToCustomerId);

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
                var invoiceWbMappingList = _invoiceWayBillMappingLogic.GetList().Where(c => c.InvoiceId == Convert.ToInt32(id));
                var invoiceToDelete = _invoiceLogic.GetSingleById(Convert.ToInt32(id));

                using (var scope = new TransactionScope())
                {
                    foreach (var item in invoiceWbMappingList)
                    {
                        _invoiceWayBillMappingLogic.Remove(item);
                    }

                    _invoiceLogic.Remove(invoiceToDelete);

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
            List<InvoiceViewModel> invoiceViewModels = new List<InvoiceViewModel>();

            var invoiceList = _invoiceLogic.GetList().Where(c => c.PaidAmount == null).OrderByDescending(c => c.InvoiceNumber).ToList();
            var invoiceWbMappingList = _invoiceWayBillMappingLogic.GetList();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            var custoemrList = _customerLogic.GetList();

            foreach (var invoice in invoiceList)
            {
                InvoiceViewModel invoiceViewModel = new InvoiceViewModel();
                invoiceViewModel.InvoiceId = invoice.Id;
                invoiceViewModel.InvoiceNumber = invoice.InvoiceNumber;
                invoiceViewModel.BillerName = custoemrList.Where(c => c.Id == invoice.BillerCustomerId).FirstOrDefault().CustomerName;
                invoiceViewModel.WayBillNumbers = invoice.WaybillNumbers;
                invoiceViewModel.TotalInvoiceAmnt = (decimal)invoice.TotalInvoiceAmount;

                invoiceViewModels.Add(invoiceViewModel);
            }

            return invoiceViewModels;

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