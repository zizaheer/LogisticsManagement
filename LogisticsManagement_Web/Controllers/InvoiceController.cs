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
                    var invoiceNo = payInfo.SelectToken("invoiceNo");
                    var billerCustomerId = payInfo.SelectToken("billerCustomerId");
                    var paidAmnt = payInfo.SelectToken("paymentAmount");
                    var bankId = payInfo.SelectToken("ddlBankId");
                    var chqNo = payInfo.SelectToken("chequeNo");
                    var chqDate = payInfo.SelectToken("chequeDate");
                    var chqAmnt = payInfo.SelectToken("chequeAmount");
                    var cashAmnt = payInfo.SelectToken("cashAmount");
                    var remarks = payInfo.SelectToken("paymentRemarks");

                    var wbInfo = (JArray)paymentData[1];
                    List<string> wbNumbers = new List<string>();

                    foreach (var item in wbInfo)
                    {
                        wbNumbers.Add(item.SelectToken("wbillNumber").ToString());
                    }

                    using (var scope = new TransactionScope())
                    {
                        var mappingList = _invoiceWayBillMappingLogic.GetList();
                        _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
                        var _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));

                        var billerAccountNo = _customerLogic.GetSingleById(Convert.ToInt32(billerCustomerId)).AccountId; // Credit Account
                        var cashAccount = _configurationLogic.GetSingleById(1).CashAccount; // Debit account 
                        var bankAccount = _configurationLogic.GetSingleById(1).BankAccount; // Debit Account

                        if(chqAmnt>0)
                        int[] debitAccounts = new int[2];
                        debitAccounts[0] = (int)bankAccount;
                        debitAccounts[1] = (int)cashAccount;

                        int[] creditAccounts = new int[1];
                        creditAccounts[0] = billerAccountNo;

                        var transactionId = MakeTransaction


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

                            waybillToUpdate.ChequeNo = chqNo.ToString();
                            waybillToUpdate.PaidAmount = waybillToUpdate.TotalWayBillAmount;
                            waybillToUpdate.PaymentDate = DateTime.Today;
                            waybillToUpdate.Remarks = remarks.ToString();

                            _invoiceWayBillMappingLogic.Update(waybillToUpdate);

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


        /// <summary>
        /// Should be able to make single debit, multiple credit and single credit multiple debit entries. Therefore debit and credit account are arrays
        /// </summary>
        /// <param name="debitAccountNo"></param>
        /// <param name="creditAccountNo"></param>
        /// <param name="transactionAmount"></param>
        /// <param name="transactionDate"></param>
        /// <param name="valueDate"></param>
        /// <param name="transactionRemarks"></param>
        /// <returns></returns>
        private int MakeTransaction(int[] debitAccountNumbers, int[] creditAccountNumbers, decimal transactionAmount, DateTime transactionDate, DateTime valueDate, string transactionRemarks)
        {

            Lms_TransactionDetailLogic _transactionDetailLogic = new Lms_TransactionDetailLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionDetailPoco>(_dbContext));
            var transactionId = _transactionDetailLogic.GetMaxId() + 1;
            Lms_TransactionDetailPoco txnDetail = new Lms_TransactionDetailPoco();
            txnDetail.Id = transactionId;
            txnDetail.TotalTransactionAmount = transactionAmount;
            txnDetail.TransactionDate = transactionDate;
            txnDetail.ValueDate = valueDate;
            txnDetail.Remarks = transactionRemarks;
            _transactionDetailLogic.Add(txnDetail);

            Lms_TransactionLogic _transactionLogic = new Lms_TransactionLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionPoco>(_dbContext));
            Lms_ChartOfAccountLogic _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));

            Lms_TransactionPoco _transactionPoco = new Lms_TransactionPoco();

            if (debitAccountNumbers.Length == 1)
            {
                //Add Debit side
                _transactionPoco = new Lms_TransactionPoco();
                _transactionPoco.Id = transactionId;
                _transactionPoco.SerialNo = 1;
                _transactionPoco.AccountId = debitAccountNumbers[0];
                _transactionPoco.TransactionAmount = transactionAmount;
                _transactionPoco.Remarks = transactionRemarks;
                _transactionLogic.Add(_transactionPoco);
                var debitAccountInfo = _chartOfAccountLogic.GetSingleById(debitAccountNumbers[0]);
                debitAccountInfo.CurrentBalance = debitAccountInfo.CurrentBalance + transactionAmount;
                _chartOfAccountLogic.Update(debitAccountInfo);

                //Add Credit side
                var serialNo = 2;
                for (int i = 0; i < creditAccountNumbers.Length; i++) {
                    _transactionPoco = new Lms_TransactionPoco();
                    _transactionPoco.Id = transactionId;
                    _transactionPoco.SerialNo = serialNo + i;
                    _transactionPoco.AccountId = creditAccountNumbers[i];
                    _transactionPoco.TransactionAmount = (-1) * transactionAmount;
                    _transactionPoco.Remarks = transactionRemarks;
                    _transactionLogic.Add(_transactionPoco);

                    var creditAccountInfo = _chartOfAccountLogic.GetSingleById(creditAccountNumbers[i]);
                    creditAccountInfo.CurrentBalance = creditAccountInfo.CurrentBalance + (-1) * transactionAmount;
                    _chartOfAccountLogic.Update(creditAccountInfo);

                }
            }

            else if (creditAccountNumbers.Length == 1)
            {
                //Add Credit side
                _transactionPoco = new Lms_TransactionPoco();
                _transactionPoco.Id = transactionId;
                _transactionPoco.SerialNo = 1;
                _transactionPoco.AccountId = creditAccountNumbers[0];
                _transactionPoco.TransactionAmount = (-1) * transactionAmount;
                _transactionPoco.Remarks = transactionRemarks;
                _transactionLogic.Add(_transactionPoco);

                var creditAccountInfo = _chartOfAccountLogic.GetSingleById(creditAccountNumbers[0]);
                creditAccountInfo.CurrentBalance = creditAccountInfo.CurrentBalance + (-1) * transactionAmount;
                _chartOfAccountLogic.Update(creditAccountInfo);

                //Add Debit side
                var serialNo = 2;
                for (int i = 0; i < debitAccountNumbers.Length; i++)
                {
                    _transactionPoco = new Lms_TransactionPoco();
                    _transactionPoco.Id = transactionId;
                    _transactionPoco.SerialNo = serialNo + i;
                    _transactionPoco.AccountId = debitAccountNumbers[i];
                    _transactionPoco.TransactionAmount = transactionAmount;
                    _transactionPoco.Remarks = transactionRemarks;
                    _transactionLogic.Add(_transactionPoco);

                    var debitAccountInfo = _chartOfAccountLogic.GetSingleById(debitAccountNumbers[i]);
                    debitAccountInfo.CurrentBalance = debitAccountInfo.CurrentBalance + transactionAmount;
                    _chartOfAccountLogic.Update(debitAccountInfo);
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