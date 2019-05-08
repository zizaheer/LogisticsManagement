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
            List<PendingWaybillsForInvoice> combineOrdersForInvoice;

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

            return pendingInvoices;

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