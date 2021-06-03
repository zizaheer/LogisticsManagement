using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ReportController : Controller
    {
        private Lms_OrderLogic _orderLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;
        private Lms_CustomerAddressMappingLogic _addressMappingLogic;
        private Lms_InvoiceLogic _invoiceLogic;
        private Lms_OrderAdditionalServiceLogic _additionalServiceLogic;
        private Lms_CompanyInfoLogic _companyInfoLogic;
        private Lms_PaymentMethodLogic _paymentMethodLogic;
        private Lms_BankLogic _bankLogic;
        private Lms_InvoicePaymentCollectionLogic _paymentCollectionLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();
        private readonly IEmailService _emailService;
        private IHostingEnvironment _hostingEnvironment;
        private IHttpContextAccessor _httpContext;

        public ReportController(IMemoryCache cache, IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContext, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _cache = cache;
            _hostingEnvironment = hostingEnvironment;
            _httpContext = httpContext;

            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            _addressMappingLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            _invoiceLogic = new Lms_InvoiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoicePoco>(_dbContext));
            _additionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
            _paymentMethodLogic = new Lms_PaymentMethodLogic(_cache, new EntityFrameworkGenericRepository<Lms_PaymentMethodPoco>(_dbContext));
            _paymentCollectionLogic = new Lms_InvoicePaymentCollectionLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoicePaymentCollectionPoco>(_dbContext));

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));
            _bankLogic = new Lms_BankLogic(_cache, new EntityFrameworkGenericRepository<Lms_BankPoco>(_dbContext));

        }

        public IActionResult Index()
        {
            ValidateSession();
            var customerList = _orderLogic.GetList();
            return View();
        }
        public IActionResult SalesReport()
        {
            ValidateSession();
            return View(GetBillToCustomers());
        }
        public IActionResult CustomerWiseDueReport()
        {
            ValidateSession();
            return View(GetBillToCustomers());
        }
        public IActionResult SalesReportGenerated([FromBody]dynamic reportParam)
        {
            DateTime _fromDate;
            DateTime _toDate;
            if (!DateTime.TryParse(((JObject)reportParam).SelectToken("startDate").ToString(), out _fromDate))
            {
                return Json("");
            }
            if (!DateTime.TryParse(((JObject)reportParam).SelectToken("endDate").ToString(), out _toDate))
            {
                return Json("");
            }

            string rptDuration = _fromDate.ToString("dd/MMM/yyyy") + " - " + _toDate.ToString("dd/MMM/yyyy");

            var customerList = ((JObject)reportParam).SelectToken("customers").ToList().Distinct();

            _toDate = _toDate.AddDays(1);

            List<ViewModel_SalesReport> viewModel_SalesReports = new List<ViewModel_SalesReport>();

            foreach (var customer in customerList)
            {

                if (string.IsNullOrEmpty(customer.ToString()))
                {
                    continue;
                }

                var orderList = _orderLogic.GetList().Where(c => c.IsInvoiced == true && c.ScheduledPickupDate >= _fromDate && c.ScheduledPickupDate < _toDate && c.BillToCustomerId == Convert.ToInt32(customer)).ToList();

                if (orderList.Count < 1)
                {
                    continue;
                }
                ViewModel_SalesReport reportData = new ViewModel_SalesReport();
                reportData.ReportDuration = rptDuration;
                reportData.BillToCustomerId = Convert.ToInt32(customer);
                reportData.BillToCustomerName = _customerLogic.GetSingleById(reportData.BillToCustomerId).CustomerName;
                reportData.TotalHst = 0;
                reportData.TotalWithHst = 0;
                reportData.TotalWithoutHst = 0;

                foreach (var order in orderList)
                {
                    decimal? orderHst = 0;

                    decimal? orderTotalWithoutHst = 0;

                    if (order.ApplicableGstPercent != null && order.ApplicableGstPercent > 0)
                    {
                        orderHst = order.TotalOrderCost - Convert.ToDecimal(Convert.ToDecimal(order.TotalOrderCost / ((100 + order.ApplicableGstPercent) / 100)).ToString("0.00"));
                        orderTotalWithoutHst = order.TotalOrderCost - orderHst;
                    }
                    else
                    {
                        orderTotalWithoutHst = order.TotalOrderCost;
                    }

                    var additionalService = _additionalServiceLogic.GetList().Where(c => c.OrderId == order.Id).ToList();

                    foreach (var addService in additionalService)
                    {
                        decimal? orderAdditionalCharge = 0;
                        decimal? orderAdditionalHst = 0;

                        if (addService.TaxAmountOnAdditionalService != null && addService.IsTaxAppliedOnAddionalService == true)
                        {
                            orderAdditionalHst = Convert.ToDecimal(Convert.ToDecimal(addService.AdditionalServiceFee * addService.TaxAmountOnAdditionalService / 100).ToString("0.00"));
                            orderHst = orderHst + orderAdditionalHst;
                        }
                        orderAdditionalCharge = addService.AdditionalServiceFee;
                        orderTotalWithoutHst = orderTotalWithoutHst + orderAdditionalCharge;
                    }

                    reportData.TotalHst = reportData.TotalHst + orderHst;
                    reportData.TotalWithoutHst = reportData.TotalWithoutHst + orderTotalWithoutHst;
                    reportData.TotalWithHst = reportData.TotalWithHst + order.TotalOrderCost + order.TotalAdditionalServiceCost;
                }

                viewModel_SalesReports.Add(reportData);
            }

            return View(viewModel_SalesReports);
        }
        public JsonResult PrintCustomerDueReportAsPdf([FromBody]dynamic reportParam)
        {
            try
            {
                var viewName = "CustomerWiseDueReportGenerated";

                var customerId = ((JObject)reportParam).SelectToken("customers").ToString();
                var isPaidIncluded = Convert.ToBoolean(((JObject)reportParam).SelectToken("isPaidIncluded").ToString());

                var _fromDateString = ((JObject)reportParam).SelectToken("startDate").ToString();
                var _toDateString = ((JObject)reportParam).SelectToken("endDate").ToString();

                DateTime _fromDate = DateTime.Now;
                DateTime _toDate = DateTime.Now;
                if (DateTime.TryParse(_fromDateString, out _fromDate))
                {
                    //code
                }
                else
                {
                    _fromDate = DateTime.Now.AddMonths(-1);
                }
                if (!DateTime.TryParse(_toDateString, out _toDate))
                {
                    //code
                }

                _companyInfoLogic = new Lms_CompanyInfoLogic(_cache, new EntityFrameworkGenericRepository<Lms_CompanyInfoPoco>(_dbContext));
                var companyInfo = _companyInfoLogic.GetSingleById(1);
                if (companyInfo != null)
                {
                    SessionData.CompanyName = !string.IsNullOrEmpty(companyInfo.CompanyName) ? companyInfo.CompanyName : "";
                    SessionData.CompanyLogo = companyInfo.CompanyLogo != null ? Convert.ToBase64String(companyInfo.CompanyLogo) : null;
                    SessionData.CompanyAddress = !string.IsNullOrEmpty(companyInfo.MainAddress) ? companyInfo.MainAddress.ToUpper() : "";
                    SessionData.CompanyTelephone = !string.IsNullOrEmpty(companyInfo.Telephone) ? companyInfo.Telephone : "";
                    SessionData.CompanyFax = companyInfo.Fax;
                    SessionData.CompanyEmail = !string.IsNullOrEmpty(companyInfo.EmailAddress) ? companyInfo.EmailAddress : "";
                    SessionData.CompanyTaxNumber = !string.IsNullOrEmpty(companyInfo.TaxNumber) ? companyInfo.TaxNumber : "";
                }

                string rptDuration = _fromDate.ToString("dd/MMM/yyyy") + " - " + _toDate.ToString("dd/MMM/yyyy");
                _toDate = _toDate.AddDays(1);



                ViewModel_Report_CustomerDue viewModel_Report_CustomerDue = new ViewModel_Report_CustomerDue();

                if (string.IsNullOrEmpty(customerId))
                {
                    return Json("");
                }

                var customerInfo = _customerLogic.GetSingleById(Convert.ToInt32(customerId));
                viewModel_Report_CustomerDue.CustomerId = customerInfo.Id;
                viewModel_Report_CustomerDue.CustomerName = customerInfo.CustomerName;
                viewModel_Report_CustomerDue.TotalAmountPayable = 0;
                viewModel_Report_CustomerDue.InvoiceDueDays = customerInfo.InvoiceDueDays;

                var addressMappingInfo = _addressMappingLogic.GetList().Where(c => c.CustomerId == customerInfo.Id && c.AddressTypeId == (byte)Enum_AddressType.Billing).FirstOrDefault();
                var addressInfo = _addressLogic.GetSingleById(addressMappingInfo.AddressId);

                viewModel_Report_CustomerDue.AddressLine1 = addressInfo.UnitNumber + " " + addressInfo.AddressLine;

                viewModel_Report_CustomerDue.City = _cityLogic.GetSingleById(addressInfo.CityId).CityName;
                viewModel_Report_CustomerDue.ProvinceCode = _provinceLogic.GetSingleById(addressInfo.ProvinceId).ShortCode;
                viewModel_Report_CustomerDue.PostCode = addressInfo.PostCode;

                var invoices = _invoiceLogic.GetList().Where(c => c.BillerCustomerId == Convert.ToInt32(customerId) && c.CreateDate >= _fromDate && c.CreateDate <= _toDate).ToList();

                if (isPaidIncluded == false)
                {
                    invoices = invoices.Where(c => c.PaidAmount == null || c.PaidAmount < c.TotalInvoiceAmount).ToList();
                }

                if (invoices.Count < 1)
                {
                    return Json("");
                }

                var paymentCollections = _paymentCollectionLogic.GetList();
                var paymentMethods = _paymentMethodLogic.GetList();
                var banks = _bankLogic.GetList();

                List<ViewModel_InvoiceDueData> invoiceDataList = new List<ViewModel_InvoiceDueData>();
                foreach (var invoice in invoices)
                {
                    ViewModel_InvoiceDueData invoiceDueData = new ViewModel_InvoiceDueData();

                    invoiceDueData.InvoiceNo = invoice.Id;
                    invoiceDueData.InvoiceDate = invoice.CreateDate.ToString("M/d/yyyy");
                    invoiceDueData.TotalInvoiceAmount = invoice.TotalInvoiceAmount;
                    invoiceDueData.PaidAmount = invoice.PaidAmount == null ? 0 : invoice.PaidAmount;
                    invoiceDueData.BalanceDueAmount = invoice.TotalInvoiceAmount - invoiceDueData.PaidAmount;

                    var paymentCollectionList = paymentCollections.Where(c => c.InvoiceId == invoice.Id).ToList();
                    if (paymentCollectionList.Count > 0)
                    {
                        var paymentCollection = paymentCollectionList.LastOrDefault();
                        if (paymentCollection != null)
                        {
                            invoiceDueData.PaymentMethod = paymentMethods.Where(c => c.Id == paymentCollection.PaymentMethodId).FirstOrDefault().MethodName;
                            invoiceDueData.PaymentDate = paymentCollection.PaymentDate.ToString("M/d/yyyy");
                            decimal chqAmount = 0;
                            foreach (var coll in paymentCollectionList)
                            {
                                chqAmount += coll.ChequeAmount != null ? (decimal)coll.ChequeAmount : 0;
                            }
                            invoiceDueData.ChqDate = paymentCollection.ChequeDate != null ? Convert.ToDateTime(paymentCollection.ChequeDate).ToString("M/d/yyyy") : "";
                            invoiceDueData.ChqNo = paymentCollection.ChequeNo;
                            invoiceDueData.ChqAmount = chqAmount.ToString("0.00");
                            invoiceDueData.BankShortName = banks.Where(c => c.Id == paymentCollection.BankId).FirstOrDefault().BankShortName;
                        }
                    }

                    invoiceDataList.Add(invoiceDueData);

                    viewModel_Report_CustomerDue.TotalAmountPayable += invoiceDueData.BalanceDueAmount;
                }

                viewModel_Report_CustomerDue.InvoiceData = invoiceDataList;

                var webrootPath = _hostingEnvironment.WebRootPath;
                var uniqueId = DateTime.Now.ToFileTime();
                var fileName = "CustomerDueReport_" + uniqueId + ".pdf";
                var directoryPath = webrootPath + "/contents/reports/";
                var filePath = directoryPath + fileName;

                if (!System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                }

                var pdfReport = new ViewAsPdf(viewName, viewModel_Report_CustomerDue)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.Letter
                };
                var file = pdfReport.BuildFile(ControllerContext).Result;

                System.IO.File.WriteAllBytes(filePath, file);

                string returnPath = "/contents/reports/" + fileName;

                //_emailService.SendEmail("zizaheer@yahoo.com", "test subject", "test body content", returnPath);

                return Json(returnPath);
            }
            catch (Exception ex)
            {
                return null;
            }
            //return View();
        }
        private List<Lms_CustomerPoco> GetBillToCustomers()
        {
            var customerList = _customerLogic.GetList();
            var billToCustomers = _invoiceLogic.GetList();

            var custList = (from customer in customerList
                            join billToCustomer in billToCustomers on customer.Id equals billToCustomer.BillerCustomerId
                            select customer).ToList().Distinct().ToList();

            return custList;
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