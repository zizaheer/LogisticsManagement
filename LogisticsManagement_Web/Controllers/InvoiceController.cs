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
        private Lms_CustomerAddressMappingLogic _customerAddressMappingLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_UnitTypeLogic _unitTypeLogic;
        private Lms_WeightScaleLogic _weightScaleLogic;

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

            return View(GetPendingWaybillsForInvoice());
        }

        public IActionResult InvoicePayment()
        {
            ValidateSession();

            return View(GetCustomersWtihPendingInvoice());
        }


        private List<ViewModel_CustomerWithPendingInvoice> GetCustomersWtihPendingInvoice()
        {

            List<ViewModel_CustomerWithPendingInvoice> customersWithPendingInvoice = new List<ViewModel_CustomerWithPendingInvoice>();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            var customers = _customerLogic.GetList();

            _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
            var addressMapping = _customerAddressMappingLogic.GetList();

            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            var addresses = _addressLogic.GetList();

            var pendingInvoices = _invoiceLogic.GetList().Where(c => c.PaidAmount == null || c.PaidAmount < c.TotalInvoiceAmount).OrderBy(c => c.BillerCustomerId).ToList();

            foreach (var customer in pendingInvoices.ToList())
            {

                ViewModel_CustomerWithPendingInvoice customerWithPendingInvoice = new ViewModel_CustomerWithPendingInvoice();

                customerWithPendingInvoice.CustomerId = customer.BillerCustomerId;

                var customerInfo = _customerLogic.GetSingleById(customer.BillerCustomerId);
                customerWithPendingInvoice.CustomerName = customerInfo.CustomerName;

                var addressMappInfo = _customerAddressMappingLogic.GetList().Where(c => c.AddressTypeId == 1 && c.CustomerId == customer.BillerCustomerId).FirstOrDefault();
                if (addressMappInfo != null)
                {
                    var addressInfo = _addressLogic.GetSingleById(addressMappInfo.Id);

                    customerWithPendingInvoice.CustomerAdress = addressInfo.UnitNumber + " " + addressInfo.AddressLine + " " + addressInfo.PostCode;
                    customerWithPendingInvoice.CustomerPhone = addressInfo.PrimaryPhoneNumber;
                }

                var sameCustomer = pendingInvoices.Where(c => c.BillerCustomerId == customerWithPendingInvoice.CustomerId).ToList();
                customerWithPendingInvoice.NumberOfInvoices = sameCustomer.Count();

                decimal totalDue = 0;
                foreach (var item in sameCustomer)
                {
                    if (item.TotalInvoiceAmount != null && item.TotalInvoiceAmount > 0)
                    {
                        if (item.PaidAmount != null && item.PaidAmount > 0)
                        {
                            totalDue += (decimal)item.TotalInvoiceAmount - (decimal)item.PaidAmount;
                        }
                        else
                        {
                            totalDue += (decimal)item.TotalInvoiceAmount;
                        }
                    }

                }

                customerWithPendingInvoice.TotalDue = totalDue;
                if (totalDue > 0)
                {
                    customersWithPendingInvoice.Add(customerWithPendingInvoice);
                }

                pendingInvoices.RemoveAll(c => c.BillerCustomerId == customerWithPendingInvoice.CustomerId);

            }

            return customersWithPendingInvoice;
        }

        private List<ViewModel_OrderReadyForInvoice> GetPendingWaybillsForInvoice()
        {

            List<ViewModel_OrderReadyForInvoice> pendingInvoices = new List<ViewModel_OrderReadyForInvoice>();
            //var combineOrdersForInvoice = new List<ViewModel_OrderReadyForInvoice>();

            try
            {
                _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
                _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
                _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));
                var customers = _customerLogic.GetList();
                ViewBag.Customers = customers;
                var unitTypes = _unitTypeLogic.GetList();
                var weightScales = _weightScaleLogic.GetList();
                var orderList = _orderLogic.GetList();
                var orderStatusList = _orderStatusLogic.GetList();

                var orders = from order in orderList
                             join status in orderStatusList on order.Id equals status.OrderId
                             where status.IsDelivered == true && order.IsInvoiced == false
                             select order;

                //var ordersStatus = _orderStatusLogic.GetList();

                foreach (var order in orders)
                {
                    ViewModel_OrderReadyForInvoice pendingInvoice = new ViewModel_OrderReadyForInvoice();

                    pendingInvoice.OrderId = order.Id;
                    pendingInvoice.WayBillNumber = order.WayBillNumber;
                    pendingInvoice.OrderTypeId = order.OrderTypeId;
                    pendingInvoice.OrderType = order.OrderTypeId == 2 ? "Return" : "Single";
                    pendingInvoice.CustomerReferenceNo = order.ReferenceNumber;
                    pendingInvoice.WaybillDate = order.CreateDate;
                    pendingInvoice.ShipperId = (int)order.ShipperCustomerId;
                    pendingInvoice.ShipperName = customers.Where(c => c.Id == pendingInvoice.ShipperId).FirstOrDefault().CustomerName;
                    pendingInvoice.ConsigneeId = (int)order.ConsigneeCustomerId;
                    pendingInvoice.ConsigneeName = customers.Where(c => c.Id == pendingInvoice.ConsigneeId).FirstOrDefault().CustomerName;
                    pendingInvoice.UnitTypeId = order.UnitTypeId;
                    pendingInvoice.UnitTypeName = unitTypes.Where(c => c.Id == pendingInvoice.UnitTypeId).FirstOrDefault().TypeName;
                    pendingInvoice.UnitQty = order.UnitQuantity;
                    pendingInvoice.SkidQty = order.SkidQuantity;
                    pendingInvoice.WeightScaleId = order.WeightScaleId;
                    pendingInvoice.WeightShortName = weightScales.Where(c => c.Id == pendingInvoice.WeightScaleId).FirstOrDefault().ShortCode;
                    pendingInvoice.WeightTotal = order.WeightTotal;
                    pendingInvoice.BillToCustomerId = order.BillToCustomerId;
                    pendingInvoice.BillerName = customers.Where(c => c.Id == pendingInvoice.BillToCustomerId).FirstOrDefault().CustomerName;
                    //pendingInvoice.BillerEmail = customers.Where(c => c.Id == pendingInvoice.BillToCustomerId).FirstOrDefault().EmailAddress;
                    pendingInvoice.OrderedBy = order.OrderedBy;
                    pendingInvoice.TotalOrderCost = order.TotalOrderCost + (order.TotalAdditionalServiceCost == null ? 0 : (decimal)order.TotalAdditionalServiceCost);

                    //if (order.OrderTypeId == 2)
                    //{
                    //    pendingInvoice.TotalReturnOrderCost = order.TotalOrderCost + (order.TotalAdditionalServiceCost == null ? 0 : (decimal)order.TotalAdditionalServiceCost);
                    //}

                    pendingInvoices.Add(pendingInvoice);

                }


            }
            catch (Exception e)
            {

            }

            return pendingInvoices;
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewInvoicedData", GetInvoicedOrders());
        }

        [HttpPost]
        public IActionResult FilterPendingInvoiceDataTable([FromBody] dynamic filterData)
        {
            try
            {
                ValidateSession();

                var dynamicData = (JObject)filterData[0];
                var startDate = dynamicData.SelectToken("startDate").ToString();
                var toDate = dynamicData.SelectToken("toDate").ToString();
                var selectedCustomer = dynamicData.SelectToken("selectedCustomer").ToString();

                var pendingInvoiceData = GetPendingWaybillsForInvoice();
                pendingInvoiceData = pendingInvoiceData.Where(c => c.WaybillDate.Date >= Convert.ToDateTime(startDate).Date && c.WaybillDate.Date.AddDays(1).Date <= Convert.ToDateTime(toDate).Date).ToList();

                if (Convert.ToInt32(selectedCustomer) > 0)
                {
                    pendingInvoiceData.Where(c => c.BillToCustomerId == Convert.ToInt32(selectedCustomer)).ToList();
                }

                return PartialView("_PartialViewPendingData", pendingInvoiceData);

            }
            catch (Exception ex)
            {
                return Json("");
            }


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

                    var orders = _orderLogic.GetList().Where(c => c.IsInvoiced == false).ToList();
                    List<ViewModel_GeneratedInvoice> invoiceViewModels = new List<ViewModel_GeneratedInvoice>();

                    for (int i = 0; i < countArray; i++)
                    {
                        wbNumbers[i] = wayBillNumberList[i].SelectToken("wbillNumber").ToString();
                        ViewModel_GeneratedInvoice invoiceViewModel = new ViewModel_GeneratedInvoice();

                        invoiceViewModel.WayBillNumbers = wbNumbers[i];
                        invoiceViewModels.Add(invoiceViewModel);
                    }

                    orders = (from order in orders
                              join waybill in invoiceViewModels on order.WayBillNumber equals waybill.WayBillNumbers
                              select order).ToList();

                    using (var scope = new TransactionScope())
                    {
                        var customerWiseOrders = new List<Lms_OrderPoco>();

                        foreach (var item in orders.ToArray())
                        {

                            if (customerWiseOrders.Find(c => c.Id == item.Id) != null) {
                                continue;
                            }

                            customerWiseOrders = orders.Where(c => c.BillToCustomerId == item.BillToCustomerId).ToList();

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
        public IActionResult Update([FromBody]dynamic invoiceData)
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

                    var orders = _orderLogic.GetList().Where(c => c.IsInvoiced == false).ToList();
                    List<ViewModel_GeneratedInvoice> invoiceViewModels = new List<ViewModel_GeneratedInvoice>();

                    for (int i = 0; i < countArray; i++)
                    {
                        wbNumbers[i] = wayBillNumberList[i].SelectToken("wbillNumber").ToString();
                        ViewModel_GeneratedInvoice invoiceViewModel = new ViewModel_GeneratedInvoice();

                        invoiceViewModel.WayBillNumbers = wbNumbers[i];
                        invoiceViewModels.Add(invoiceViewModel);
                    }

                    orders = (from order in orders
                              join waybill in invoiceViewModels on order.WayBillNumber equals waybill.WayBillNumbers
                              select order).ToList();

                    using (var scope = new TransactionScope())
                    {
                        foreach (var item in orders.ToList())
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


        [HttpGet]
        public IActionResult FillPartialViewCustomerWiseDueInvoices(string id)
        {
            ValidateSession();
            return PartialView("_PartialViewCustomerWiseDueInvoices", GetDueInvoicesByCustomerId(id));
        }

        [HttpGet]
        public IActionResult GetDueInvoicesByCustomerId(string id)
        {
            var pendingList = new List<Lms_InvoicePoco>();
            if (id != "")
            {
                pendingList = _invoiceLogic.GetList().Where(c => c.BillerCustomerId == Convert.ToInt32(id) && c.TotalInvoiceAmount != null && c.TotalInvoiceAmount > (c.PaidAmount != null ? c.PaidAmount : 0)).ToList();
            }

            return Json(JsonConvert.SerializeObject(pendingList));
        }

        [HttpGet]
        public IActionResult GetDueWaybillsByInvoiceId(string id)
        {
            try
            {
                var mappingList = new List<Lms_InvoiceWayBillMappingPoco>();
                _invoiceWayBillMappingLogic = new Lms_InvoiceWayBillMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoiceWayBillMappingPoco>(_dbContext));
                mappingList = _invoiceWayBillMappingLogic.GetList();

                if (mappingList != null && mappingList.Count > 0)
                {
                    mappingList = mappingList.Where(c => c.InvoiceId == Convert.ToInt32(id)).ToList();

                    var orderList = _orderLogic.GetList();
                    var orderStatusList = _orderStatusLogic.GetList();

                    var wayBillList = new List<ViewModel_InvoiceWiseWayBill>();

                    foreach (var item in mappingList)
                    {

                        ViewModel_InvoiceWiseWayBill viewModel_InvoiceWiseWayBill = new ViewModel_InvoiceWiseWayBill();
                        viewModel_InvoiceWiseWayBill.WaybillNumber = item.WayBillNumber;

                        var wayBillInfo = orderList.Where(c => c.WayBillNumber == item.WayBillNumber).FirstOrDefault();
                        var wayBillStatusInfo = orderStatusList.Where(c => c.OrderId == wayBillInfo.Id).FirstOrDefault();

                        viewModel_InvoiceWiseWayBill.PickupDate = (DateTime)wayBillStatusInfo.PickupDatetime;
                        viewModel_InvoiceWiseWayBill.DeliveryDate = (DateTime)wayBillStatusInfo.DeliveredDatetime;

                        viewModel_InvoiceWiseWayBill.TotalWaybillAmount = item.TotalWayBillAmount;
                        if (wayBillInfo.ApplicableGstPercent != null && wayBillInfo.ApplicableGstPercent > 0)
                        {
                            viewModel_InvoiceWiseWayBill.TotalTaxAmount = wayBillInfo.ApplicableGstPercent * viewModel_InvoiceWiseWayBill.TotalWaybillAmount / 100;
                        }
                        else
                        {
                            viewModel_InvoiceWiseWayBill.TotalTaxAmount = 0;
                        }

                        var totalPaidAmount = item.PaidAmount != null ? item.PaidAmount : 0 + item.DiscountAmount != null ? item.DiscountAmount : 0 + item.WaivedAmount != null ? item.WaivedAmount : 0;
                        if (item.TotalWayBillAmount > totalPaidAmount)
                        {
                            viewModel_InvoiceWiseWayBill.IsCleared = false;
                        }
                        else
                        {
                            viewModel_InvoiceWiseWayBill.IsCleared = true;
                        }

                        wayBillList.Add(viewModel_InvoiceWiseWayBill);

                    }

                    return Json(JsonConvert.SerializeObject(wayBillList));
                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                return null;
            }
        }


        private List<ViewModel_GeneratedInvoice> GetInvoicedOrders()
        {
            List<ViewModel_GeneratedInvoice> invoiceViewModels = new List<ViewModel_GeneratedInvoice>();

            var invoiceList = _invoiceLogic.GetList().Where(c => c.PaidAmount == null).OrderByDescending(c => c.Id).ToList();
            var invoiceWbMappingList = _invoiceWayBillMappingLogic.GetList();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            var custoemrList = _customerLogic.GetList();

            foreach (var invoice in invoiceList)
            {
                ViewModel_GeneratedInvoice invoiceViewModel = new ViewModel_GeneratedInvoice();
                invoiceViewModel.InvoiceId = invoice.Id;
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
                    Response.Redirect("~/Login/Index");
                }
            }
            else
            {
                Response.Redirect("~/Login/InvalidLocation");
            }
        }

    }
}