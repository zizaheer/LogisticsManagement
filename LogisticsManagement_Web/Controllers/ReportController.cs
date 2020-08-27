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
    public class ReportController : Controller
    {
        private Lms_OrderLogic _orderLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_CustomerAddressMappingLogic _addressLogic;
        private Lms_InvoiceLogic _invoiceLogic;
        private Lms_OrderAdditionalServiceLogic _additionalServiceLogic;
        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public ReportController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _dbContext = dbContext;
            _cache = cache;
            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            _addressLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
            _invoiceLogic = new Lms_InvoiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoicePoco>(_dbContext));
            _additionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
        }

        public IActionResult Index()
        {
            var customerList = _orderLogic.GetList();
            return View();
        }
        public IActionResult SalesReport()
        {
            var customerList = _customerLogic.GetList();
            var addressMapping = _addressLogic.GetList();

            var custList = (from customer in customerList
                           join address in addressMapping on customer.Id equals address.CustomerId
                           where address.AddressTypeId == (short)Enum_AddressType.Billing
                           select customer).ToList().Distinct().ToList();

            return View(custList);
        }

        public IActionResult SalesReportGenerated([FromBody]dynamic reportParam)
        {
            DateTime _fromDate;
            DateTime _toDate;
            if (!DateTime.TryParse(((JObject)reportParam).SelectToken("startDate").ToString(), out _fromDate)) {
                return Json("");
            }
            if (!DateTime.TryParse(((JObject)reportParam).SelectToken("endDate").ToString(), out _toDate)) {
                return Json("");
            }

            string rptDuration = _fromDate.ToString("dd/MMM/yyyy") + " - " + _toDate.ToString("dd/MMM/yyyy");

            var customerList = ((JObject)reportParam).SelectToken("customers").ToList().Distinct();

            _toDate = _toDate.AddDays(1);

            List<ViewModel_SalesReport> viewModel_SalesReports = new List<ViewModel_SalesReport>();

            foreach (var customer in customerList) {

                if (string.IsNullOrEmpty(customer.ToString())) {
                    continue;
                }

                var orderList = _orderLogic.GetList().Where(c => c.IsInvoiced == true && c.ScheduledPickupDate >= _fromDate && c.ScheduledPickupDate < _toDate && c.BillToCustomerId== Convert.ToInt32(customer)).ToList();

                if (orderList.Count < 1) {
                    continue;
                }
                ViewModel_SalesReport reportData = new ViewModel_SalesReport();
                reportData.ReportDuration = rptDuration;
                reportData.BillToCustomerId = Convert.ToInt32(customer);
                reportData.BillToCustomerName= _customerLogic.GetSingleById(reportData.BillToCustomerId).CustomerName;
                reportData.TotalHst = 0;
                reportData.TotalWithHst = 0;
                reportData.TotalWithoutHst = 0;

                foreach (var order in orderList)
                {
                    decimal? orderHst = 0;
                    
                    decimal? orderTotalWithoutHst = 0;

                    if (order.ApplicableGstPercent != null && order.ApplicableGstPercent>0) {
                        orderHst = order.TotalOrderCost - Convert.ToDecimal(Convert.ToDecimal(order.TotalOrderCost / ((100 + order.ApplicableGstPercent) / 100)).ToString("0.00"));
                        orderTotalWithoutHst = order.TotalOrderCost - orderHst;
                    }
                    else
                    {
                        orderTotalWithoutHst = order.TotalOrderCost;
                    }

                    var additionalService = _additionalServiceLogic.GetList().Where(c => c.OrderId == order.Id).ToList();

                    foreach (var addService in additionalService) {
                        decimal? orderAdditionalCharge = 0;
                        decimal? orderAdditionalHst = 0;

                        if (addService.TaxAmountOnAdditionalService != null && addService.IsTaxAppliedOnAddionalService ==true)
                        {
                            orderAdditionalHst = Convert.ToDecimal(Convert.ToDecimal(addService.AdditionalServiceFee* addService.TaxAmountOnAdditionalService /100).ToString("0.00"));
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
    }
}