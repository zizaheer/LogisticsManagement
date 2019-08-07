using System;
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


        List<Lms_OrderAdditionalServicePoco> orderAdditionalServices = new List<Lms_OrderAdditionalServicePoco>();
        List<Lms_AdditionalServicePoco> additionalServices = new List<Lms_AdditionalServicePoco>();
        List<Lms_AddressPoco> addresses = new List<Lms_AddressPoco>();
        List<Lms_CustomerAddressMappingPoco> addressMappings = new List<Lms_CustomerAddressMappingPoco>();
        List<App_CityPoco> cities = new List<App_CityPoco>();
        List<App_ProvincePoco> provinces = new List<App_ProvincePoco>();
        List<Lms_CustomerPoco> customers = new List<Lms_CustomerPoco>();
        List<Lms_DeliveryOptionPoco> deliveryOptions = new List<Lms_DeliveryOptionPoco>();
        List<Lms_UnitTypePoco> unitTypes = new List<Lms_UnitTypePoco>();
        List<Lms_WeightScalePoco> weightScales = new List<Lms_WeightScalePoco>();

        public InvoiceController(IMemoryCache cache, IEmailService emailService, IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContext, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _emailService = emailService;
            _hostingEnvironment = hostingEnvironment;
            _httpContext = httpContext;

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
            Lms_PaymentMethodLogic paymentMethodLogic = new Lms_PaymentMethodLogic(_cache, new EntityFrameworkGenericRepository<Lms_PaymentMethodPoco>(_dbContext));
            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));

            ViewBag.Customers = _customerLogic.GetList();
            ViewBag.Banks = lms_BankLogic.GetList();
            ViewBag.PaymentMethods = paymentMethodLogic.GetList();

            return View(GetCustomersWtihPendingInvoice(0, string.Empty, false));
        }

        private List<ViewModel_CustomerWithPendingInvoice> GetCustomersWtihPendingInvoice(int customerId, string year, bool isPaidInclusive)
        {

            List<ViewModel_CustomerWithPendingInvoice> customersWithPendingInvoice = new List<ViewModel_CustomerWithPendingInvoice>();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            var customers = _customerLogic.GetList();

            _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
            var addressMapping = _customerAddressMappingLogic.GetList();

            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            var addresses = _addressLogic.GetList();

            List<Lms_InvoicePoco> pendingInvoices = new List<Lms_InvoicePoco>();

            pendingInvoices = _invoiceLogic.GetList();
            if (customerId > 0)
            {
                pendingInvoices = pendingInvoices.Where(c => c.BillerCustomerId == customerId).ToList();
            }

            if (year != "")
            {
                if (Convert.ToInt16(year) > 2000)
                {
                    pendingInvoices = pendingInvoices.Where(c => c.CreateDate.ToString("yyyy") == year).ToList();
                }
            }

            if (isPaidInclusive == false)
            {
                pendingInvoices = pendingInvoices.Where(c => c.PaidAmount == null || c.PaidAmount < c.TotalInvoiceAmount).ToList();
            }

            pendingInvoices = pendingInvoices.OrderBy(c => c.BillerCustomerId).ToList();

            foreach (var customer in pendingInvoices.ToList())
            {
                ViewModel_CustomerWithPendingInvoice customerWithPendingInvoice = new ViewModel_CustomerWithPendingInvoice();

                customerWithPendingInvoice.CustomerId = customer.BillerCustomerId;

                var customerInfo = _customerLogic.GetSingleById(customer.BillerCustomerId);
                customerWithPendingInvoice.CustomerName = customerInfo.CustomerName;

                var addressMappInfo = new Lms_CustomerAddressMappingPoco();
                addressMappInfo = _customerAddressMappingLogic.GetList().Where(c => c.AddressTypeId == 1 && c.CustomerId == customer.BillerCustomerId).FirstOrDefault();
                var addressInfo = new Lms_AddressPoco();

                if (addressMappInfo != null)
                {
                    addressInfo = _addressLogic.GetSingleById(addressMappInfo.AddressId);
                    addressInfo.UnitNumber = addressInfo.UnitNumber == null ? "" : addressInfo.UnitNumber;
                    customerWithPendingInvoice.CustomerAdress = addressInfo.UnitNumber + " " + addressInfo.AddressLine + " " + addressInfo.PostCode;
                    customerWithPendingInvoice.CustomerPhone = addressInfo.PrimaryPhoneNumber;
                }
                else
                {
                    addressMappInfo = _customerAddressMappingLogic.GetList().Where(c => c.AddressTypeId == 2 && c.CustomerId == customer.BillerCustomerId).FirstOrDefault();
                    if (addressMappInfo != null)
                    {
                        addressInfo = _addressLogic.GetSingleById(addressMappInfo.AddressId);
                        addressInfo.UnitNumber = addressInfo.UnitNumber == null ? "" : addressInfo.UnitNumber;
                        customerWithPendingInvoice.CustomerAdress = addressInfo.UnitNumber + " " + addressInfo.AddressLine + " " + addressInfo.PostCode;
                        customerWithPendingInvoice.CustomerPhone = addressInfo.PrimaryPhoneNumber;
                    }
                    else
                    {
                        addressMappInfo = _customerAddressMappingLogic.GetList().Where(c => c.AddressTypeId == 4 && c.CustomerId == customer.BillerCustomerId).FirstOrDefault();
                        if (addressMappInfo != null)
                        {
                            addressInfo = _addressLogic.GetSingleById(addressMappInfo.AddressId);
                            addressInfo.UnitNumber = addressInfo.UnitNumber == null ? "" : addressInfo.UnitNumber;
                            customerWithPendingInvoice.CustomerAdress = addressInfo.UnitNumber + " " + addressInfo.AddressLine + " " + addressInfo.PostCode;
                            customerWithPendingInvoice.CustomerPhone = addressInfo.PrimaryPhoneNumber;
                        }
                    }
                }

                var sameCustomer = pendingInvoices.Where(c => c.BillerCustomerId == customerWithPendingInvoice.CustomerId).ToList();
                customerWithPendingInvoice.NumberOfInvoices = sameCustomer.Count();

                decimal totalDue = 0;
                foreach (var item in sameCustomer)
                {
                    if (item.TotalInvoiceAmount > 0)
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
                customersWithPendingInvoice.Add(customerWithPendingInvoice);

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
                    if (order.UnitQuantity != null && order.UnitQuantity > 0)
                    {
                        pendingInvoice.UnitTypeId = order.UnitTypeId;
                        pendingInvoice.UnitTypeName = unitTypes.Where(c => c.Id == pendingInvoice.UnitTypeId).FirstOrDefault().TypeName;
                        pendingInvoice.UnitQty = order.UnitQuantity;
                    }

                    pendingInvoice.SkidQty = order.SkidQuantity;

                    if (order.WeightTotal > 0)
                    {
                        if (pendingInvoice.WeightScaleId > 0)
                        {
                            pendingInvoice.WeightScaleId = order.WeightScaleId;
                            pendingInvoice.WeightShortName = weightScales.Where(c => c.Id == pendingInvoice.WeightScaleId).FirstOrDefault().ShortCode;
                            pendingInvoice.WeightTotal = order.WeightTotal;
                        }
                    }

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
        public IActionResult PartialViewDataTable(string id)
        {
            ValidateSession();
            var isMiscellaneous = false;
            if (id != "")
            {
                isMiscellaneous = Convert.ToBoolean(id);
            }
            return PartialView("_PartialViewInvoicedData", GetInvoicedOrders(isMiscellaneous));
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

        [HttpGet]
        public IActionResult PartialCustomersInvoiceDueDataTable(string customerId, string year, string isPaid)
        {
            ValidateSession();
            int billerId = 0;
            string selectedYear = "";
            bool isPaidInclusive = false;

            if (customerId != "")
                billerId = Convert.ToInt32(customerId);
            if (year != "")
                selectedYear = year;
            if (isPaid != "")
                isPaidInclusive = Convert.ToBoolean(Convert.ToInt16(isPaid));

            return PartialView("_PartialViewCustomersInvoiceDue", GetCustomersWtihPendingInvoice(billerId, selectedYear, isPaidInclusive));

        }


        [HttpGet]
        public IActionResult PartialGetPaidInvoicesByCustomer(string customerId, string isPaid)
        {
            ValidateSession();
            int billerId = 0;
            bool isPaidInclusive = false;

            if (customerId.Length > 0)
                billerId = Convert.ToInt32(customerId);
            if (isPaid != "")
                isPaidInclusive = Convert.ToBoolean(Convert.ToInt16(isPaid));

            var invoiceList = _invoiceLogic.GetList().Where(c => c.PaidAmount == c.TotalInvoiceAmount).ToList();
            if (billerId > 0) {
                invoiceList = invoiceList.Where(c => c.BillerCustomerId == billerId).ToList();
            }

            return PartialView("_PartialViewCustomerWiseDueInvoices", invoiceList);

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
                if (id != "")
                {
                    var invoiceInfo = _invoiceLogic.GetSingleById(Convert.ToInt32(id));
                    var invoiceWbMappingList = _invoiceWayBillMappingLogic.GetList().Where(c => c.InvoiceId == Convert.ToInt32(id));

                    var paidAmount = invoiceInfo.PaidAmount;

                    using (var scope = new TransactionScope())
                    {
                        foreach (var item in invoiceWbMappingList)
                        {
                            item.TotalWayBillAmount = 0;

                            _invoiceWayBillMappingLogic.Update(item);
                        }

                        if (paidAmount != null && paidAmount > 0)
                        {

                        }

                        _invoiceLogic.Remove(invoiceInfo);

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
                    var invoiceNo = Convert.ToInt32(payInfo.SelectToken("invoiceNo"));
                    var billerCustomerId = Convert.ToInt32(payInfo.SelectToken("billerCustomerId"));
                    var paidAmnt = Convert.ToDecimal(payInfo.SelectToken("paymentAmount"));
                    var paymentMethodId = Convert.ToInt32(payInfo.SelectToken("paymentMethodId"));
                    var bankId = Convert.ToInt32(payInfo.SelectToken("ddlBankId"));
                    var chqNo = payInfo.SelectToken("chequeNo").ToString();
                    var chqDate = payInfo.SelectToken("chequeDate").ToString();
                    var chqAmnt = Convert.ToDecimal(payInfo.SelectToken("chequeAmount"));
                    var cashAmnt = Convert.ToDecimal(payInfo.SelectToken("cashAmount"));
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
                        var _paymentCollectionLogic = new Lms_InvoicePaymentCollectionLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoicePaymentCollectionPoco>(_dbContext));

                        var billerAccountNo = _customerLogic.GetSingleById(Convert.ToInt32(billerCustomerId)).AccountId; // Credit Account
                        var cashAccount = _configurationLogic.GetSingleById(1).CashAccount; // Debit account 
                        var bankAccount = _configurationLogic.GetSingleById(1).BankAccount; // Debit Account


                        var creditTxnInfoList = new List<TransactionModel>();
                        var creditTxnInfo = new TransactionModel();
                        creditTxnInfo.AccountId = billerAccountNo;
                        creditTxnInfo.TxnAmount = Convert.ToDecimal(paidAmnt);
                        creditTxnInfoList.Add(creditTxnInfo);

                        var debitTxnInfoList = new List<TransactionModel>();
                        if (chqAmnt > 0 && cashAmnt > 0)
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
                        else if (chqAmnt > 0 && cashAmnt <= 0)
                        {
                            var debitTxnInfo = new TransactionModel();
                            debitTxnInfo.AccountId = (int)bankAccount;
                            debitTxnInfo.TxnAmount = Convert.ToDecimal(chqAmnt);
                            debitTxnInfoList.Add(debitTxnInfo);
                        }
                        else if (chqAmnt <= 0 && cashAmnt > 0)
                        {
                            var debitTxnInfo = new TransactionModel();
                            debitTxnInfo.AccountId = (int)cashAccount;
                            debitTxnInfo.TxnAmount = Convert.ToDecimal(cashAmnt);
                            debitTxnInfoList.Add(debitTxnInfo);
                        }

                        transactionId = MakeTransaction(debitTxnInfoList, creditTxnInfoList, Convert.ToDecimal(paidAmnt), DateTime.Today, DateTime.Today, remarks);

                        var totalPaymentReceived = Convert.ToDecimal(paidAmnt);
                        foreach (var item in wbNumbers)
                        {
                            var waybillToUpdate = mappingList.Where(c => c.InvoiceId == Convert.ToInt32(invoiceNo) && c.WayBillNumber == item && c.IsClear == false).FirstOrDefault();

                            if (waybillToUpdate != null)
                            {
                                if (totalPaymentReceived >= waybillToUpdate.TotalWayBillAmount)
                                {
                                    waybillToUpdate.IsClear = true;
                                    _invoiceWayBillMappingLogic.Update(waybillToUpdate);
                                    totalPaymentReceived = totalPaymentReceived - waybillToUpdate.TotalWayBillAmount;
                                }
                            }
                        }

                        Lms_InvoicePaymentCollectionPoco invoicePaymentCollection = new Lms_InvoicePaymentCollectionPoco();
                        invoicePaymentCollection.InvoiceId = Convert.ToInt32(invoiceNo);
                        invoicePaymentCollection.PaymentAmount = Convert.ToDecimal(paidAmnt);
                        invoicePaymentCollection.PaymentMethodId = Convert.ToInt16(paymentMethodId);
                        invoicePaymentCollection.PaymentDate = DateTime.Today;
                        invoicePaymentCollection.TransactionId = transactionId;
                        invoicePaymentCollection.CashAmount = cashAmnt > 0 ? cashAmnt : (decimal?)null;
                        invoicePaymentCollection.ChequeAmount = chqAmnt > 0 ? chqAmnt : (decimal?)null;
                        invoicePaymentCollection.ChequeNo = chqNo;
                        invoicePaymentCollection.ChequeDate = chqDate != "" ? Convert.ToDateTime(chqDate) : (DateTime?)null;
                        invoicePaymentCollection.BankId = bankId > 0 ? bankId : (int?)null;
                        invoicePaymentCollection.Remarks = remarks;

                        _paymentCollectionLogic.Add(invoicePaymentCollection);

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

        [HttpPost]
        public IActionResult UndoInvoicing(string id)
        {
            ValidateSession();
            var result = "";

            Lms_InvoicePoco invoicePoco = new Lms_InvoicePoco();

            try
            {
                if (id != "")
                {
                    invoicePoco = _invoiceLogic.GetSingleById(Convert.ToInt32(id));
                }

                if (invoicePoco != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        Lms_TransactionLogic _transactionLogic = new Lms_TransactionLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionPoco>(_dbContext));
                        Lms_TransactionDetailLogic _transactionDetailLogic = new Lms_TransactionDetailLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionDetailPoco>(_dbContext));
                        Lms_ChartOfAccountLogic _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));
                        Lms_InvoicePaymentCollectionLogic _invoicePaymentCollectionLogic = new Lms_InvoicePaymentCollectionLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoicePaymentCollectionPoco>(_dbContext));
                        _invoiceWayBillMappingLogic = new Lms_InvoiceWayBillMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoiceWayBillMappingPoco>(_dbContext));

                        int debitAccountId = 0;
                        int creditAccountId = 0;

                        var tranDetailInfo = _transactionDetailLogic.GetSingleById((int)invoicePoco.InvoiceGenTxnId);
                        if (tranDetailInfo != null)
                        {
                            _transactionDetailLogic.Remove(tranDetailInfo);

                            var transInfo = _transactionLogic.GetList().Where(c => c.Id == invoicePoco.InvoiceGenTxnId).ToList();
                            if (transInfo != null)
                            {
                                debitAccountId = transInfo.Where(c => c.TransactionAmount > 0).FirstOrDefault().AccountId; // identifies which account was debited; this account now has to be credited to adjust txn amount
                                creditAccountId = transInfo.Where(c => c.TransactionAmount < 0).FirstOrDefault().AccountId;
                                foreach (var tran in transInfo)
                                {
                                    _transactionLogic.Remove(tran);
                                }

                                var invoiceMappingInfo = _invoiceWayBillMappingLogic.GetList().Where(c => c.InvoiceId == invoicePoco.Id).ToList();
                                foreach (var mapping in invoiceMappingInfo)
                                {
                                    mapping.TotalWayBillAmount = 0;
                                    _invoiceWayBillMappingLogic.Update(mapping);

                                    //var orderList = _orderLogic.GetList().Where(c => c.WayBillNumber == mapping.WayBillNumber).ToList();
                                    //foreach (var order in orderList) {
                                    //    order.IsInvoiced = false;
                                    //    _orderLogic.Update(order);
                                    //}
                                }

                                var debitAccountInfo = _chartOfAccountLogic.GetSingleById(debitAccountId);
                                debitAccountInfo.CurrentBalance = debitAccountInfo.CurrentBalance - invoicePoco.TotalInvoiceAmount;
                                _chartOfAccountLogic.Update(debitAccountInfo);

                                var creditAccountInfo = _chartOfAccountLogic.GetSingleById(creditAccountId);
                                creditAccountInfo.CurrentBalance = creditAccountInfo.CurrentBalance + invoicePoco.TotalInvoiceAmount;
                                _chartOfAccountLogic.Update(creditAccountInfo);

                                var paymentCollectionInfo = _invoicePaymentCollectionLogic.GetList().Where(c => c.InvoiceId == invoicePoco.Id).ToList();
                                foreach (var collection in paymentCollectionInfo)
                                {
                                    tranDetailInfo = new Lms_TransactionDetailPoco();
                                    tranDetailInfo = _transactionDetailLogic.GetSingleById(collection.TransactionId);
                                    if (tranDetailInfo != null)
                                    {
                                        _transactionDetailLogic.Remove(tranDetailInfo);

                                        transInfo = _transactionLogic.GetList().Where(c => c.Id == tranDetailInfo.Id).ToList();
                                        foreach (var tran in transInfo)
                                        {
                                            if (tran.TransactionAmount > 0)
                                            {
                                                debitAccountId = tran.AccountId;
                                                debitAccountInfo = _chartOfAccountLogic.GetSingleById(debitAccountId);
                                                debitAccountInfo.CurrentBalance = debitAccountInfo.CurrentBalance - tran.TransactionAmount;
                                                _chartOfAccountLogic.Update(debitAccountInfo);
                                            }
                                            else
                                            {
                                                creditAccountId = tran.AccountId;
                                                creditAccountInfo = _chartOfAccountLogic.GetSingleById(creditAccountId);
                                                creditAccountInfo.CurrentBalance = creditAccountInfo.CurrentBalance + tran.TransactionAmount;
                                                _chartOfAccountLogic.Update(creditAccountInfo);
                                            }

                                            _transactionLogic.Remove(tran);
                                        }
                                    }

                                    _invoicePaymentCollectionLogic.Remove(collection);
                                }

                                invoicePoco.InvoiceGenTxnId = null;
                                invoicePoco.TotalInvoiceAmount = 0;
                                _invoiceLogic.Update(invoicePoco);
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
        public IActionResult RegenerateInvoice(string id)
        {
            ValidateSession();
            var result = "";

            Lms_InvoicePoco invoicePoco = new Lms_InvoicePoco();

            try
            {
                if (id != "")
                {
                    invoicePoco = _invoiceLogic.GetSingleById(Convert.ToInt32(id));
                }

                if (invoicePoco != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        Lms_TransactionLogic _transactionLogic = new Lms_TransactionLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionPoco>(_dbContext));
                        Lms_TransactionDetailLogic _transactionDetailLogic = new Lms_TransactionDetailLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionDetailPoco>(_dbContext));
                        Lms_ChartOfAccountLogic _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));
                        _invoiceWayBillMappingLogic = new Lms_InvoiceWayBillMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoiceWayBillMappingPoco>(_dbContext));
                        _invoiceLogic = new Lms_InvoiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoicePoco>(_dbContext));

                        _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));

                        _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                        var configInfo = _configurationLogic.GetSingleById(1);
                        _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));


                        decimal totalInvoiceAmount = 0;
                        var waybillInfoList = _invoiceWayBillMappingLogic.GetList().Where(c => c.InvoiceId == invoicePoco.Id).ToList();
                        foreach (var waybill in waybillInfoList)
                        {

                            decimal orderPrice = 0;
                            var orderInfo = _orderLogic.GetList().Where(c => c.WayBillNumber == waybill.WayBillNumber).ToList();
                            foreach (var order in orderInfo)
                            {
                                if (order.OrderTypeId == 3)
                                {
                                    orderPrice = (decimal)order.TotalAdditionalServiceCost;
                                }
                                else
                                {
                                    orderPrice = orderPrice + order.TotalOrderCost + (decimal)order.TotalAdditionalServiceCost;
                                }
                            }
                            totalInvoiceAmount = totalInvoiceAmount + orderPrice;
                            waybill.TotalWayBillAmount = orderPrice;
                            waybill.IsClear = false;
                            _invoiceWayBillMappingLogic.Update(waybill);

                        }

                        int debitAccountId = _customerLogic.GetSingleById(invoicePoco.BillerCustomerId).AccountId;
                        int creditAccountId = (int)configInfo.SalesIncomeAccount;

                        List<TransactionModel> debitAccountDetailList = new List<TransactionModel>();
                        TransactionModel debitAccountDetail = new TransactionModel();
                        debitAccountDetail.AccountId = debitAccountId;
                        debitAccountDetail.TxnAmount = totalInvoiceAmount;

                        debitAccountDetailList.Add(debitAccountDetail);

                        List<TransactionModel> creditAccountDetailList = new List<TransactionModel>();
                        TransactionModel creditAccountDetail = new TransactionModel();
                        creditAccountDetail.AccountId = creditAccountId;
                        creditAccountDetail.TxnAmount = totalInvoiceAmount;
                        creditAccountDetailList.Add(creditAccountDetail);

                        var transactionId = MakeTransaction(debitAccountDetailList, creditAccountDetailList, totalInvoiceAmount, DateTime.Now, DateTime.Now, "Invoice re-generated");

                        var invoiceInfo = _invoiceLogic.GetSingleById(invoicePoco.Id);
                        invoiceInfo.InvoiceGenTxnId = transactionId;
                        invoiceInfo.TotalInvoiceAmount = totalInvoiceAmount;
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

                        viewModel_InvoiceWiseWayBill.IsCleared = item.IsClear;

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

        public JsonResult PrintInvoiceAsPdf([FromBody]dynamic printData)
        {

            ValidateSession();
            var result = "";

            var viewModelPrintInvoice = new ViewModel_PrintInvoice();
            var viewName = "";

            try
            {
                if (printData != null)
                {
                    // parsing data
                    List<ViewModel_GeneratedInvoice> invoiceViewModels = new List<ViewModel_GeneratedInvoice>();
                    List<Lms_OrderPoco> orders = new List<Lms_OrderPoco>();
                    var invoiceBillerList = new List<ViewModel_InvoiceBiller>();
                    var invoiceWaybillList = new List<ViewModel_PrintWaybill>();
                    var billerList = new List<int>();
                    orders = _orderLogic.GetList();
                    GetRequiredData();

                    var printOptionObject = (JObject)printData[1];
                    var isMiscellaneous = Convert.ToBoolean(printOptionObject.SelectToken("isMiscellaneous"));
                    viewName = printOptionObject.SelectToken("viewName").ToString();
                    var isFinalPrint = Convert.ToBoolean(printOptionObject.SelectToken("isFinalPrint"));

                    var countArray = 0;
                    var dataObject = new JObject();
                    string[] itemArray = new string[0];

                    dataObject = (JObject)printData[0];
                    if (dataObject != null)
                    {
                        if (isFinalPrint == false)
                        {
                            var wayBillNumberList = dataObject.SelectToken("wayBillNumberArray");
                            countArray = ((JArray)wayBillNumberList).Count;
                            itemArray = new string[countArray];

                            for (int i = 0; i < countArray; i++)
                            {
                                itemArray[i] = wayBillNumberList[i].SelectToken("wbillNumber").ToString();
                                ViewModel_GeneratedInvoice invoiceViewModel = new ViewModel_GeneratedInvoice();

                                invoiceViewModel.WayBillNumbers = itemArray[i];
                                invoiceViewModels.Add(invoiceViewModel);
                            }

                            //orders = orders.Where(c => c.IsInvoiced == false).ToList();
                            orders = (from order in orders
                                      join waybill in invoiceViewModels on order.WayBillNumber equals waybill.WayBillNumbers
                                      where order.IsInvoiced == false
                                      select order).ToList();

                            billerList = orders.Select(c => c.BillToCustomerId).Distinct().ToList();

                            foreach (var billerId in billerList)
                            {
                                var biller = GetBillerInformation(billerId);
                                invoiceBillerList.Add(biller);
                            }

                            foreach (var item in orders)
                            {
                                var waybillInfoForPrint = GetWaybillInformationForPrint(item, isMiscellaneous);
                                invoiceWaybillList.Add(waybillInfoForPrint);
                            }

                        }
                        else
                        {
                            var invoiceList = dataObject.SelectToken("invoiceNumberArray");
                            countArray = ((JArray)invoiceList).Count;
                            itemArray = new string[countArray];

                            for (int i = 0; i < countArray; i++)
                            {
                                var invNo = invoiceList[i].SelectToken("invoiceNumber").ToString();
                                var invoice = _invoiceLogic.GetSingleById(Convert.ToInt32(invNo));
                                var biller = GetBillerInformation(invoice.BillerCustomerId);
                                biller.InvoiceNo = Convert.ToInt32(invNo);
                                biller.InvoiceDate = invoice.CreateDate;

                                invoiceBillerList.Add(biller);

                                var orderWaybillNumbers = _invoiceWayBillMappingLogic.GetList().Where(c => c.InvoiceId == Convert.ToInt32(invNo)).Select(d => d.WayBillNumber).ToList();
                                foreach (var waybill in orderWaybillNumbers)
                                {
                                    var orderInfo = orders.Where(c => c.WayBillNumber == waybill).ToList();
                                    foreach (var order in orderInfo)
                                    {
                                        if (order.OrderTypeId == 3)
                                        {
                                            isMiscellaneous = true;
                                            viewName = "PrintMiscellaneousInvoice";
                                        }
                                        else
                                        {
                                            isMiscellaneous = false;
                                            viewName = "PrintDeliveryInvoice";
                                        }
                                        var waybillInfoForPrint = GetWaybillInformationForPrint(order, isMiscellaneous);
                                        waybillInfoForPrint.InvoiceNumber = Convert.ToInt32(invNo);
                                        invoiceWaybillList.Add(waybillInfoForPrint);
                                    }
                                }
                            }
                        }

                        viewModelPrintInvoice.viewModelInvoiceBillers = invoiceBillerList;
                        viewModelPrintInvoice.viewModelWaybills = invoiceWaybillList;
                    }

                }

                var webrootPath = _hostingEnvironment.WebRootPath;
                var uniqueId = DateTime.Now.ToFileTime();
                var fileName = "invoice_" + uniqueId + ".pdf";
                var directoryPath = webrootPath + "/contents/invoices/";
                var filePath = directoryPath + fileName;

                if (!System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                }

                var pdfReport = new ViewAsPdf(viewName, viewModelPrintInvoice)
                {
                    CustomSwitches = "--page-offset 0 --footer-center [page]/[toPage] --footer-font-size 8"
                };

                var file = pdfReport.BuildFile(ControllerContext).Result;
                System.IO.File.WriteAllBytes(filePath, file);

                string returnPath = "/contents/invoices/" + fileName;

                //_emailService.SendEmail("zizaheer@yahoo.com", "test subject", "test body content", returnPath);

                return Json(returnPath);
            }

            catch (Exception ex)
            {

            }

            return Json(result);
        }

        private ViewModel_InvoiceBiller GetBillerInformation(int billerCustomerId)
        {
            ViewModel_InvoiceBiller invoiceBiller = new ViewModel_InvoiceBiller();

            invoiceBiller.BillerCustomerId = billerCustomerId;
            //invoiceBiller.BillerDepartment = billerInfo.DepartmentName;
            invoiceBiller.BillerCustomerName = customers.Where(c => c.Id == billerCustomerId).FirstOrDefault().CustomerName;
            invoiceBiller.Term = customers.Where(c => c.Id == billerCustomerId).FirstOrDefault().InvoiceDueDays;

            var addressInfo = addressMappings.Where(c => c.AddressTypeId == 2 && c.CustomerId == billerCustomerId).FirstOrDefault(); // first checking whether there is a billing address
            if (addressInfo == null)
            {
                addressInfo = addressMappings.Where(c => c.AddressTypeId == 1 && c.CustomerId == billerCustomerId).FirstOrDefault(); // if not, second checking whether there is a shipping address
            }
            if (addressInfo == null)
            {
                addressInfo = addressMappings.Where(c => c.AddressTypeId == 4 && c.CustomerId == billerCustomerId).FirstOrDefault(); // if not, third checking whether there is a warehouse address
            }
            if (addressInfo != null)
            {
                var billerAddressInfo = addresses.Where(c => c.Id == addressInfo.AddressId).FirstOrDefault();
                invoiceBiller.BillerCustomerAddressLine = string.IsNullOrEmpty(billerAddressInfo.UnitNumber) ? billerAddressInfo.AddressLine : billerAddressInfo.UnitNumber + ", " + billerAddressInfo.AddressLine;
                invoiceBiller.BillerCustomerCityLine = cities.Where(c => c.Id == billerAddressInfo.CityId).FirstOrDefault().CityName + ", ";
                invoiceBiller.BillerCustomerCityLine += provinces.Where(c => c.Id == billerAddressInfo.ProvinceId).FirstOrDefault().ShortCode;
                invoiceBiller.BillerPostCode = billerAddressInfo.PostCode;
            }

            return invoiceBiller;
        }
        private ViewModel_PrintWaybill GetWaybillInformationForPrint(Lms_OrderPoco order, bool isMiscellaneous)
        {

            ViewModel_PrintWaybill waybillPrintViewModel = new ViewModel_PrintWaybill();

            if (order != null)
            {
                waybillPrintViewModel.WaybillNumber = order.WayBillNumber;
                waybillPrintViewModel.WayBillDate = order.CreateDate.ToString("dd-MMM-yy");
                waybillPrintViewModel.BillerCustomerId = order.BillToCustomerId;
                waybillPrintViewModel.CustomerRefNo = order.ReferenceNumber;
                waybillPrintViewModel.CargoCtlNo = order.CargoCtlNumber;
                waybillPrintViewModel.AwbContainerNo = order.AwbCtnNumber;
                waybillPrintViewModel.BillerCustomerName = customers.Where(c => c.Id == order.BillToCustomerId).FirstOrDefault().CustomerName;
                waybillPrintViewModel.OrderedByName = order.OrderedBy;
                if (order.DeliveryOptionId != null && order.DeliveryOptionId > 0)
                {
                    waybillPrintViewModel.DeliveryOptionShortCode = deliveryOptions.Where(c => c.Id == order.DeliveryOptionId).FirstOrDefault().ShortCode;
                }

                waybillPrintViewModel.OrderBasePrice = order.OrderBasicCost.ToString();
                if (order.BasicCostOverriden != null && order.BasicCostOverriden > 0)
                {
                    waybillPrintViewModel.OrderBasePrice = order.BasicCostOverriden.ToString();
                }

                waybillPrintViewModel.FuelSurcharge = "0.00";
                if (order.FuelSurchargePercentage != null && order.FuelSurchargePercentage > 0)
                {
                    var fuelAmnt = Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) * order.FuelSurchargePercentage / 100;
                    if (fuelAmnt > 0)
                    {
                        waybillPrintViewModel.FuelSurcharge = Convert.ToDecimal(fuelAmnt).ToString("0.00");
                    }
                }

                waybillPrintViewModel.OrderDiscountAmount = "0.00";
                if (order.DiscountPercentOnOrderCost != null && order.DiscountPercentOnOrderCost > 0)
                {
                    var orderDiscount = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) + Convert.ToDecimal(waybillPrintViewModel.FuelSurcharge)) * order.DiscountPercentOnOrderCost / 100;
                    if (orderDiscount > 0)
                    {
                        waybillPrintViewModel.OrderDiscountAmount = Convert.ToDecimal(orderDiscount).ToString();
                        //waybillPrintViewModel.OrderBasePrice = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) - (decimal)orderDiscount).ToString("0.00");
                    }
                }

                decimal orderTotalTax = 0; // for misc. orders
                waybillPrintViewModel.AdditionalServicesComments = "";
                waybillPrintViewModel.AdditionalServiceCost = "0.00";
                if (order.TotalAdditionalServiceCost > 0)
                {
                    waybillPrintViewModel.AdditionalServiceCost = Convert.ToDecimal(order.TotalAdditionalServiceCost).ToString("0.00");

                    var addServices = orderAdditionalServices.Where(c => c.OrderId == order.Id).ToList();
                    foreach (var addservice in addServices)
                    {
                        var serviceCode = additionalServices.Where(c => c.Id == addservice.AdditionalServiceId).FirstOrDefault().ServiceCode;
                        var serviceCost = addservice.AdditionalServiceFee;

                        if (isMiscellaneous == true)
                        {
                            if (order.DiscountPercentOnOrderCost != null && order.DiscountPercentOnOrderCost > 0)
                            {
                                serviceCost = serviceCost - (serviceCost * (decimal)order.DiscountPercentOnOrderCost / 100);
                            }
                        }

                        if (addservice.IsTaxAppliedOnAddionalService == true && addservice.TaxAmountOnAdditionalService != null && addservice.TaxAmountOnAdditionalService > 0)
                        {
                            orderTotalTax += serviceCost * (decimal)addservice.TaxAmountOnAdditionalService / 100;
                            waybillPrintViewModel.AdditionalServicesComments = waybillPrintViewModel.AdditionalServicesComments + ("*" + serviceCode + ": " + addservice.AdditionalServiceFee.ToString("0.00") + "; ");
                        }
                        else
                        {
                            waybillPrintViewModel.AdditionalServicesComments = waybillPrintViewModel.AdditionalServicesComments + (serviceCode + ": " + addservice.AdditionalServiceFee.ToString("0.00") + "; ");
                        }
                    }

                }

                waybillPrintViewModel.OrderTaxAmountOnBasePrice = "0.00";

                if (isMiscellaneous == true)
                {
                    waybillPrintViewModel.OrderTaxAmountOnBasePrice = Convert.ToDecimal(orderTotalTax).ToString("0.00");
                    waybillPrintViewModel.NetTotalOrderCost = Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice).ToString("0.00");
                }
                else
                {
                    if (order.ApplicableGstPercent != null && order.ApplicableGstPercent > 0)
                    {
                        var taxAmnt = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) + Convert.ToDecimal(waybillPrintViewModel.FuelSurcharge) - Convert.ToDecimal(waybillPrintViewModel.OrderDiscountAmount)) * order.ApplicableGstPercent / 100;
                        if (taxAmnt > 0)
                        {
                            waybillPrintViewModel.OrderTaxAmountOnBasePrice = Convert.ToDecimal(taxAmnt).ToString("0.00");
                        }
                    }
                    waybillPrintViewModel.NetTotalOrderCost = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) + Convert.ToDecimal(waybillPrintViewModel.FuelSurcharge) + Convert.ToDecimal(waybillPrintViewModel.AdditionalServiceCost)).ToString("0.00");
                }

                waybillPrintViewModel.ShipperCustomerName = customers.Where(c => c.Id == order.ShipperCustomerId).FirstOrDefault().CustomerName;
                var shippperAddress = addresses.Where(c => c.Id == order.ShipperAddressId).FirstOrDefault();
                waybillPrintViewModel.ShipperCustomerAddressLine1 = !string.IsNullOrEmpty(shippperAddress.UnitNumber) ? shippperAddress.UnitNumber + ", " + shippperAddress.AddressLine : shippperAddress.AddressLine;
                waybillPrintViewModel.ShipperCustomerAddressLine2 = cities.Where(c => c.Id == shippperAddress.CityId).FirstOrDefault().CityName + ", " + provinces.Where(c => c.Id == shippperAddress.ProvinceId).FirstOrDefault().ShortCode + "  " + shippperAddress.PostCode;

                if (isMiscellaneous == false)
                {
                    waybillPrintViewModel.ConsigneeCustomerName = customers.Where(c => c.Id == order.ConsigneeCustomerId).FirstOrDefault().CustomerName;
                    var consigneeAddress = addresses.Where(c => c.Id == order.ConsigneeAddressId).FirstOrDefault();
                    waybillPrintViewModel.ConsigneeCustomerAddressLine1 = !string.IsNullOrEmpty(consigneeAddress.UnitNumber) ? consigneeAddress.UnitNumber + ", " + consigneeAddress.AddressLine : consigneeAddress.AddressLine;
                    waybillPrintViewModel.ConsigneeCustomerAddressLine2 = cities.Where(c => c.Id == consigneeAddress.CityId).FirstOrDefault().CityName + ", " + provinces.Where(c => c.Id == consigneeAddress.ProvinceId).FirstOrDefault().ShortCode + "  " + consigneeAddress.PostCode;
                }

                waybillPrintViewModel.SkidQuantity = order.SkidQuantity;
                waybillPrintViewModel.TotalSkidPieces = order.TotalPiece;

                if (order.UnitQuantity > 0)
                {
                    waybillPrintViewModel.UnitQuantity = order.UnitQuantity;
                    if (order.UnitTypeId > 0)
                    {
                        waybillPrintViewModel.UnitTypeName = unitTypes.Where(c => c.Id == order.UnitTypeId).FirstOrDefault().TypeName;
                        waybillPrintViewModel.UnitTypeShortCode = unitTypes.Where(c => c.Id == order.UnitTypeId).FirstOrDefault().ShortCode;
                    }
                }

                if (order.WeightTotal > 0)
                {
                    waybillPrintViewModel.WeightTotal = order.WeightTotal.ToString();
                    if (order.WeightScaleId > 0)
                    {
                        waybillPrintViewModel.WeightScaleShortCode = weightScales.Where(c => c.Id == order.WeightScaleId).FirstOrDefault().ShortCode;
                    }
                }

                //waybillPrintViewModel.DeliveryDate = null;
                //waybillPrintViewModel.DeliveryTime = null;

                waybillPrintViewModel.WaybillComments = "";
                waybillPrintViewModel.InvoiceComments = "";
                if (order.IsPrintedOnWayBill != null && order.IsPrintedOnWayBill == true)
                {
                    waybillPrintViewModel.WaybillComments = order.CommentsForWayBill;
                }

                if (order.IsPrintedOnInvoice != null && order.IsPrintedOnInvoice == true)
                {
                    waybillPrintViewModel.InvoiceComments = "Notes: " + order.CommentsForInvoice;
                }

            }

            return waybillPrintViewModel;
        }

        private void GetRequiredData()
        {
            // required data
            _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
            orderAdditionalServices = _orderAdditionalServiceLogic.GetList();
            _additionalServiceLogic = new Lms_AdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>(_dbContext));
            additionalServices = _additionalServiceLogic.GetList();
            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            addresses = _addressLogic.GetList();
            _customerAddressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
            addressMappings = _customerAddressMappingLogic.GetList();
            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            cities = _cityLogic.GetList();
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            provinces = _provinceLogic.GetList();
            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            customers = _customerLogic.GetList();
            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(_cache, new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            deliveryOptions = _deliveryOptionLogic.GetList();
            _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
            unitTypes = _unitTypeLogic.GetList();
            _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));
            weightScales = _weightScaleLogic.GetList();
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
                if (_transactionPoco.TransactionAmount > 0)
                {
                    _transactionLogic.Add(_transactionPoco);
                    var debitAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                    debitAccount.CurrentBalance = debitAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                    _chartOfAccountLogic.Update(debitAccount);
                }

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
                    if (_transactionPoco.TransactionAmount < 0)
                    {
                        _transactionLogic.Add(_transactionPoco);
                        var creditAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                        creditAccount.CurrentBalance = creditAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                        _chartOfAccountLogic.Update(creditAccount);
                    }
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
                if (_transactionPoco.TransactionAmount < 0)
                {
                    _transactionLogic.Add(_transactionPoco);
                    var creditAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                    creditAccount.CurrentBalance = creditAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                    _chartOfAccountLogic.Update(creditAccount);
                }
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
                    if (_transactionPoco.TransactionAmount > 0)
                    {
                        _transactionLogic.Add(_transactionPoco);

                        var debitAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                        debitAccount.CurrentBalance = debitAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                        _chartOfAccountLogic.Update(debitAccount);
                    }
                }
            }

            return transactionId;
        }


        private List<ViewModel_GeneratedInvoice> GetInvoicedOrders(bool isMisc)
        {
            List<ViewModel_GeneratedInvoice> invoiceViewModels = new List<ViewModel_GeneratedInvoice>();

            var invoiceList = _invoiceLogic.GetList().Where(c => c.PaidAmount == null).OrderByDescending(c => c.Id).ToList();
            var invoiceWbMappingList = _invoiceWayBillMappingLogic.GetList();
            var orderList = _orderLogic.GetList();

            if (isMisc)
            {
                invoiceList = (from invoice in invoiceList
                               join mapping in invoiceWbMappingList on invoice.Id equals mapping.InvoiceId
                               join order in orderList on mapping.WayBillNumber equals order.WayBillNumber
                               where order.OrderTypeId == 3
                               select invoice).Distinct().ToList();
            }
            else
            {
                invoiceList = (from invoice in invoiceList
                               join mapping in invoiceWbMappingList on invoice.Id equals mapping.InvoiceId
                               join order in orderList on mapping.WayBillNumber equals order.WayBillNumber
                               where order.OrderTypeId != 3
                               select invoice).Distinct().ToList();
            }

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