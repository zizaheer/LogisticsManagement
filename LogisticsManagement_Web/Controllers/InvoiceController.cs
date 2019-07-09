﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
using LogisticsManagement_Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rotativa.AspNetCore;

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
       
        private Lms_EmployeeLogic _employeeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private Lms_OrderAdditionalServiceLogic _orderAdditionalServiceLogic;
        private Lms_AdditionalServiceLogic _additionalServiceLogic;
        private Lms_ConfigurationLogic _configurationLogic;

        private readonly LogisticsContext _dbContext;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();
        private readonly IEmailService _emailService;
        private IHostingEnvironment _hostingEnvironment;
        private IHttpContextAccessor _httpContext;
        int orderTypeToLoad = 0;

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
            orderTypeToLoad = 1;
            return View(GetPendingWaybillsForInvoice());
        }

        public IActionResult InvoicePayment()
        {
            ValidateSession();
            Lms_BankLogic lms_BankLogic = new Lms_BankLogic(_cache, new EntityFrameworkGenericRepository<Lms_BankPoco>(_dbContext));
            ViewBag.Banks = lms_BankLogic.GetList();
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

                var orders = new List<Lms_OrderPoco>();

                if (orderTypeToLoad == 1)
                {
                    orders = (from order in orderList
                              join status in orderStatusList on order.Id equals status.OrderId
                              where status.IsDelivered == true && order.IsInvoiced == false
                              select order).ToList();
                }
                else if (orderTypeToLoad == 3)
                {
                    orders = orderList.Where(c => c.IsInvoiced == false && c.OrderTypeId == 3).ToList();
                }

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
                    if (order.ConsigneeCustomerId != null)
                    {
                        pendingInvoice.ConsigneeId = (int)order.ConsigneeCustomerId;
                        pendingInvoice.ConsigneeName = customers.Where(c => c.Id == pendingInvoice.ConsigneeId).FirstOrDefault().CustomerName;
                    }
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

        [HttpGet]
        public IActionResult FilterPendingInvoiceDataTable(string filterData)
        {
            try
            {
                ValidateSession();

                var desrializedObject = (JObject)(JsonConvert.DeserializeObject(filterData));
                var startDate = desrializedObject.SelectToken("startDate").ToString();
                var toDate = desrializedObject.SelectToken("toDate").ToString();
                var selectedCustomer = desrializedObject.SelectToken("selectedCustomer").ToString();
                var orderType = desrializedObject.SelectToken("orderType").ToString();

                if (orderType != "")
                {
                    orderTypeToLoad = Convert.ToInt16(orderType);
                }
                else
                {
                    orderTypeToLoad = 1;
                }

                var pendingInvoiceData = GetPendingWaybillsForInvoice();
                pendingInvoiceData = pendingInvoiceData.Where(c => c.WaybillDate.Date >= Convert.ToDateTime(startDate).Date && c.WaybillDate.Date <= Convert.ToDateTime(toDate).Date.AddDays(1)).ToList();

                int outCustomerId = 0;
                if (int.TryParse(selectedCustomer, out outCustomerId))
                {
                    if (outCustomerId > 0)
                    {
                        pendingInvoiceData = pendingInvoiceData.Where(c => c.BillToCustomerId == outCustomerId).ToList();
                    }
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

                            if (customerWiseOrders.Find(c => c.Id == item.Id) != null)
                            {
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




        // Invoice payments
        [HttpPost]
        public IActionResult MakePayment([FromBody]dynamic paymentData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (paymentData != null)
                {
                    var payInfo = (JObject)paymentData[0];
                    var invoiceNo = payInfo.SelectToken("invoiceNo").ToString();
                    var billerCustomerId = payInfo.SelectToken("billerCustomerId").ToString();
                    var paidAmnt = payInfo.SelectToken("paymentAmount").ToString();
                    var bankId = payInfo.SelectToken("ddlBankId").ToString();
                    var chqNo = payInfo.SelectToken("chequeNo").ToString();
                    var chqDate = payInfo.SelectToken("chequeDate").ToString();
                    var chqAmnt = payInfo.SelectToken("chequeAmount").ToString();
                    var cashAmnt = payInfo.SelectToken("cashAmount").ToString();
                    var remarks = payInfo.SelectToken("paymentRemarks").ToString();

                    var wbInfo = (JArray)paymentData[1];
                    List<string> wbNumbers = new List<string>();

                    foreach (var item in wbInfo)
                    {
                        wbNumbers.Add(item.SelectToken("wbillNumber").ToString());
                    }

                    using (var scope = new TransactionScope())
                    {
                        var transactionId = 0;

                        var mappingList = _invoiceWayBillMappingLogic.GetList();
                        _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
                        var _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));

                        var billerAccountNo = _customerLogic.GetSingleById(Convert.ToInt32(billerCustomerId)).AccountId; // Credit Account
                        var cashAccount = _configurationLogic.GetSingleById(1).CashAccount; // Debit account 
                        var bankAccount = _configurationLogic.GetSingleById(1).BankAccount; // Debit Account


                        var creditTxnInfoList = new List<TransactionModel>();
                        var creditTxnInfo = new TransactionModel();
                        creditTxnInfo.AccountId = billerAccountNo;
                        creditTxnInfo.TxnAmount = Convert.ToDecimal(paidAmnt);
                        creditTxnInfoList.Add(creditTxnInfo);

                        var debitTxnInfoList = new List<TransactionModel>();
                        if (!string.IsNullOrEmpty(chqAmnt) && !string.IsNullOrEmpty(cashAmnt))
                        {
                            var debitTxnInfo = new TransactionModel();
                            debitTxnInfo.AccountId = (int)bankAccount;
                            debitTxnInfo.TxnAmount = Convert.ToDecimal(chqAmnt);
                            debitTxnInfoList.Add(debitTxnInfo);
                            debitTxnInfo = new TransactionModel();
                            debitTxnInfo.AccountId = (int)cashAccount;
                            debitTxnInfo.TxnAmount = Convert.ToDecimal(cashAmnt);
                            debitTxnInfoList.Add(debitTxnInfo);
                        }
                        else if (!string.IsNullOrEmpty(chqAmnt) && string.IsNullOrEmpty(cashAmnt))
                        {
                            var debitTxnInfo = new TransactionModel();
                            debitTxnInfo.AccountId = (int)bankAccount;
                            debitTxnInfo.TxnAmount = Convert.ToDecimal(chqAmnt);
                            debitTxnInfoList.Add(debitTxnInfo);
                        }
                        else if (string.IsNullOrEmpty(chqAmnt) && !string.IsNullOrEmpty(cashAmnt))
                        {
                            var debitTxnInfo = new TransactionModel();
                            debitTxnInfo.AccountId = (int)cashAccount;
                            debitTxnInfo.TxnAmount = Convert.ToDecimal(cashAmnt);
                            debitTxnInfoList.Add(debitTxnInfo);
                        }

                        transactionId = MakeTransaction(debitTxnInfoList, creditTxnInfoList, Convert.ToDecimal(paidAmnt), DateTime.Today, DateTime.Today, remarks);

                        foreach (var item in wbNumbers)
                        {
                            var waybillToUpdate = mappingList.Where(c => c.InvoiceId == Convert.ToInt32(invoiceNo) && c.WayBillNumber == item).FirstOrDefault();

                            if (bankId.ToString() != "")
                                waybillToUpdate.BankId = Convert.ToInt16(bankId);

                            if (cashAmnt.ToString() != "")
                                waybillToUpdate.CashAmount = Convert.ToDecimal(cashAmnt);

                            if (chqAmnt.ToString() != "")
                                waybillToUpdate.ChequeAmount = Convert.ToDecimal(chqAmnt);

                            if (chqDate.ToString() != "")
                                waybillToUpdate.ChequeDate = Convert.ToDateTime(chqDate);

                            waybillToUpdate.TransactionId = transactionId;
                            waybillToUpdate.ChequeNo = chqNo.ToString();
                            waybillToUpdate.PaidAmount = waybillToUpdate.TotalWayBillAmount;
                            waybillToUpdate.PaymentDate = DateTime.Today;
                            waybillToUpdate.Remarks = remarks.ToString();

                            _invoiceWayBillMappingLogic.Update(waybillToUpdate);

                        }

                        var invoiceInfo = _invoiceLogic.GetSingleById(Convert.ToInt32(invoiceNo));
                        invoiceInfo.PaidAmount = invoiceInfo.PaidAmount == null ? 0 + Convert.ToDecimal(paidAmnt) : invoiceInfo.PaidAmount + Convert.ToDecimal(paidAmnt);
                        _invoiceLogic.Update(invoiceInfo);

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


                        if (wayBillStatusInfo != null)
                        {
                            if (wayBillStatusInfo.PickupDatetime != null)
                            {
                                viewModel_InvoiceWiseWayBill.PickupDate = (DateTime)wayBillStatusInfo.PickupDatetime;
                            }
                            if (wayBillStatusInfo.DeliveredDatetime != null)
                            {
                                viewModel_InvoiceWiseWayBill.DeliveryDate = (DateTime)wayBillStatusInfo.DeliveredDatetime;
                            }
                        }

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

        public JsonResult PrintInvoice([FromBody]dynamic orderData)
        {
            try
            {
                List<ViewModel_PrintWaybill> waybillPrintViewModels = new List<ViewModel_PrintWaybill>();
                JArray wayBillNumberList = null;

                var orderList = _orderLogic.GetList();

                _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                var addresses = _addressLogic.GetList();

                _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
                var cities = _cityLogic.GetList();

                _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
                var provinces = _provinceLogic.GetList();

                _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
                var customers = _customerLogic.GetList();

                _deliveryOptionLogic = new Lms_DeliveryOptionLogic(_cache, new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
                var deliveryOptions = _deliveryOptionLogic.GetList();

                _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
                var unitTypes = _unitTypeLogic.GetList();

                _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));
                var weightScales = _weightScaleLogic.GetList();

                if (orderData != null)
                {
                    wayBillNumberList = JArray.Parse(JsonConvert.SerializeObject(orderData[0]));
                }

                foreach (var item in wayBillNumberList)
                {
                    var wbNumber = item.ToString();
                    var orderInfo = orderList.Where(c => c.WayBillNumber == wbNumber && c.OrderTypeId == 1).FirstOrDefault(); //consider only the single order; not the return order
                    if (orderInfo != null)
                    {
                        ViewModel_PrintWaybill waybillPrintViewModel = new ViewModel_PrintWaybill();

                        waybillPrintViewModel.WaybillNumber = orderInfo.WayBillNumber;
                        waybillPrintViewModel.WayBillDate = orderInfo.CreateDate.ToString("dd-MMM-yy");
                        waybillPrintViewModel.BillerCustomerId = orderInfo.BillToCustomerId;
                        waybillPrintViewModel.CustomerRefNo = orderInfo.ReferenceNumber;
                        waybillPrintViewModel.CargoCtlNo = orderInfo.CargoCtlNumber;
                        waybillPrintViewModel.AwbContainerNo = orderInfo.AwbCtnNumber;
                        waybillPrintViewModel.BillerCustomerName = customers.Where(c => c.Id == orderInfo.BillToCustomerId).FirstOrDefault().CustomerName;
                        waybillPrintViewModel.OrderedByName = orderInfo.OrderedBy;
                        waybillPrintViewModel.DeliveryOptionShortCode = deliveryOptions.Where(c => c.Id == orderInfo.DeliveryOptionId).FirstOrDefault().ShortCode;

                        waybillPrintViewModel.OrderBasePrice = orderInfo.OrderBasicCost.ToString();
                        if (orderInfo.BasicCostOverriden != null && orderInfo.BasicCostOverriden > 0)
                        {
                            waybillPrintViewModel.OrderBasePrice = orderInfo.BasicCostOverriden.ToString();
                        }

                        if (orderInfo.DiscountPercentOnOrderCost != null && orderInfo.DiscountPercentOnOrderCost > 0)
                        {
                            waybillPrintViewModel.OrderBasePrice = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) - (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) * orderInfo.DiscountPercentOnOrderCost / 100)).ToString();
                        }

                        if (orderInfo.FuelSurchargePercentage != null && orderInfo.FuelSurchargePercentage > 0)
                        {
                            waybillPrintViewModel.FuelSurcharge = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) * orderInfo.FuelSurchargePercentage / 100).ToString();
                        }

                        waybillPrintViewModel.AdditionalServiceCost = orderInfo.TotalAdditionalServiceCost.ToString();

                        if (orderInfo.ApplicableGstPercent != null && orderInfo.ApplicableGstPercent > 0)
                        {
                            waybillPrintViewModel.OrderTaxAmount = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) * orderInfo.ApplicableGstPercent / 100).ToString();
                        }

                        waybillPrintViewModel.TotalOrderCost = orderInfo.TotalOrderCost.ToString();
                        waybillPrintViewModel.PickupFromCustomerName = customers.Where(c => c.Id == orderInfo.ShipperCustomerId).FirstOrDefault().CustomerName;

                        var shippperAddress = addresses.Where(c => c.Id == orderInfo.ShipperAddressId).FirstOrDefault();

                        waybillPrintViewModel.PickupFromCustomerAddressLine1 = !string.IsNullOrEmpty(shippperAddress.UnitNumber) ? shippperAddress.UnitNumber + ", " + shippperAddress.AddressLine : shippperAddress.AddressLine;
                        waybillPrintViewModel.PickupFromCustomerAddressLine2 = cities.Where(c => c.Id == shippperAddress.CityId).FirstOrDefault().CityName + ", " + provinces.Where(c => c.Id == shippperAddress.ProvinceId).FirstOrDefault().ShortCode + "  " + shippperAddress.PostCode;

                        waybillPrintViewModel.DeliveredToCustomerName = customers.Where(c => c.Id == orderInfo.ConsigneeCustomerId).FirstOrDefault().CustomerName;

                        var consigneeAddress = addresses.Where(c => c.Id == orderInfo.ConsigneeAddressId).FirstOrDefault();


                        waybillPrintViewModel.DeliveredToCustomerAddressLine1 = !string.IsNullOrEmpty(consigneeAddress.UnitNumber) ? consigneeAddress.UnitNumber + ", " + consigneeAddress.AddressLine : consigneeAddress.AddressLine;
                        waybillPrintViewModel.DeliveredToCustomerAddressLine2 = cities.Where(c => c.Id == consigneeAddress.CityId).FirstOrDefault().CityName + ", " + provinces.Where(c => c.Id == consigneeAddress.ProvinceId).FirstOrDefault().ShortCode + "  " + consigneeAddress.PostCode;

                        waybillPrintViewModel.TotalSkidPieces = 0;
                        waybillPrintViewModel.UnitTypeName = unitTypes.Where(c => c.Id == orderInfo.UnitTypeId).FirstOrDefault().TypeName;
                        waybillPrintViewModel.UnitTypeShortCode = unitTypes.Where(c => c.Id == orderInfo.UnitTypeId).FirstOrDefault().ShortCode;
                        waybillPrintViewModel.UnitQuantity = orderInfo.UnitQuantity;
                        waybillPrintViewModel.WeightScaleShortCode = weightScales.Where(c => c.Id == orderInfo.WeightScaleId).FirstOrDefault().ShortCode;
                        waybillPrintViewModel.WeightTotal = orderInfo.WeightTotal.ToString();
                        waybillPrintViewModel.DeliveryDate = null;
                        waybillPrintViewModel.DeliveryTime = null;
                        waybillPrintViewModel.PUDriverName = "";
                        waybillPrintViewModel.DeliveryDriverName = orderInfo.WayBillNumber;
                        if (orderInfo.IsPrintedOnWayBill != null && orderInfo.IsPrintedOnWayBill == true)
                        {
                            waybillPrintViewModel.WaybillComments = orderInfo.CommentsForWayBill;
                        }

                        waybillPrintViewModels.Add(waybillPrintViewModel);

                    }
                }

                var webrootPath = _hostingEnvironment.WebRootPath;
                var uniqueId = DateTime.Now.ToFileTime();
                var path = "/contents/invoices/invoice_" + uniqueId + ".pdf";
                var filePath = webrootPath + path;

                var pdfReport = new ViewAsPdf("PrintInvoice", waybillPrintViewModels);
                var file = pdfReport.BuildFile(ControllerContext).Result;

                System.IO.File.WriteAllBytes(filePath, file);

                //_emailService.SendEmail("zizaheer@yahoo.com", "test subject", "test body content", path);


                return Json(path);
            }
            catch (Exception ex)
            {
                return null;
            }
            //return View();
        }


        /// <summary>
        ///  Should be able to make single debit, multiple credit and single credit multiple debit entries. Therefore debit and credit info are passed as objects
        /// </summary>
        /// <param name="debitAccountInfo"></param>
        /// <param name="creditAccountInfo"></param>
        /// <param name="transactionAmount"></param>
        /// <param name="transactionDate"></param>
        /// <param name="valueDate"></param>
        /// <param name="transactionRemarks"></param>
        /// <returns></returns>
        private int MakeTransaction(List<TransactionModel> debitAccountInfo, List<TransactionModel> creditAccountInfo, decimal transactionAmount, DateTime transactionDate, DateTime valueDate, string transactionRemarks)
        {

            Lms_TransactionDetailLogic _transactionDetailLogic = new Lms_TransactionDetailLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionDetailPoco>(_dbContext));
            var transactionId = _transactionDetailLogic.GetMaxId() + 1;
            Lms_TransactionDetailPoco txnDetail = new Lms_TransactionDetailPoco();
            txnDetail.Id = transactionId;
            txnDetail.TransactionAmount = transactionAmount;
            txnDetail.TransactionDate = transactionDate;
            txnDetail.ValueDate = valueDate;
            txnDetail.Remarks = transactionRemarks;
            _transactionDetailLogic.Add(txnDetail);

            Lms_TransactionLogic _transactionLogic = new Lms_TransactionLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionPoco>(_dbContext));
            Lms_ChartOfAccountLogic _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));

            Lms_TransactionPoco _transactionPoco = new Lms_TransactionPoco();

            if (debitAccountInfo.Count == 1)
            {
                //Add Debit side
                _transactionPoco = new Lms_TransactionPoco();
                _transactionPoco.Id = transactionId;
                _transactionPoco.SerialNo = 1;
                _transactionPoco.AccountId = debitAccountInfo.FirstOrDefault().AccountId;
                _transactionPoco.TransactionAmount = debitAccountInfo.FirstOrDefault().TxnAmount;
                _transactionPoco.Remarks = transactionRemarks;
                _transactionLogic.Add(_transactionPoco);
                var debitAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                debitAccount.CurrentBalance = debitAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                _chartOfAccountLogic.Update(debitAccount);

                //Add Credit side
                var serialNo = 2;
                for (int i = 0; i < creditAccountInfo.Count; i++)
                {
                    _transactionPoco = new Lms_TransactionPoco();
                    _transactionPoco.Id = transactionId;
                    _transactionPoco.SerialNo = serialNo + i;
                    _transactionPoco.AccountId = creditAccountInfo[i].AccountId;
                    _transactionPoco.TransactionAmount = (-1) * creditAccountInfo[i].TxnAmount;
                    _transactionPoco.Remarks = transactionRemarks;
                    _transactionLogic.Add(_transactionPoco);

                    var creditAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                    creditAccount.CurrentBalance = creditAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                    _chartOfAccountLogic.Update(creditAccount);

                }
            }

            else if (creditAccountInfo.Count == 1)
            {
                //Add Credit side
                _transactionPoco = new Lms_TransactionPoco();
                _transactionPoco.Id = transactionId;
                _transactionPoco.SerialNo = 1;
                _transactionPoco.AccountId = creditAccountInfo.FirstOrDefault().AccountId;
                _transactionPoco.TransactionAmount = (-1) * creditAccountInfo.FirstOrDefault().TxnAmount;
                _transactionPoco.Remarks = transactionRemarks;
                _transactionLogic.Add(_transactionPoco);

                var creditAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                creditAccount.CurrentBalance = creditAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                _chartOfAccountLogic.Update(creditAccount);

                //Add Debit side
                var serialNo = 2;
                for (int i = 0; i < debitAccountInfo.Count; i++)
                {
                    _transactionPoco = new Lms_TransactionPoco();
                    _transactionPoco.Id = transactionId;
                    _transactionPoco.SerialNo = serialNo + i;
                    _transactionPoco.AccountId = debitAccountInfo[i].AccountId;
                    _transactionPoco.TransactionAmount = debitAccountInfo[i].TxnAmount;
                    _transactionPoco.Remarks = transactionRemarks;
                    _transactionLogic.Add(_transactionPoco);

                    var debitAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                    debitAccount.CurrentBalance = debitAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                    _chartOfAccountLogic.Update(debitAccount);
                }
            }

            return transactionId;
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
                    Response.Redirect(Request.Host + "Login/Index");
                }
            }
            else
            {
                Response.Redirect("Login/InvalidLocation");
            }
        }

    }
}