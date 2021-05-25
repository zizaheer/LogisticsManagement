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

namespace LogisticsManagement_Web.Controllers
{
    public class SearchController : Controller
    {

        private readonly LogisticsContext _dbContext;
        private Lms_OrderLogic _orderLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_EmployeeLogic _employeeLogic;

        private Lms_AddressLogic _addressLogic;
        private Lms_UnitTypeLogic _unitTypeLogic;
        private Lms_WeightScaleLogic _weightScaleLogic;

        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private Lms_VehicleTypeLogic _vehicleTypeScaleLogic;

        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private App_CountryLogic _countryLogic;

        IMemoryCache _cache;
        SessionData sessionData = new SessionData();
        private readonly IEmailService _emailService;
        private IHostingEnvironment _hostingEnvironment;
        private IHttpContextAccessor _httpContext;

        public SearchController(IMemoryCache cache, IEmailService emailService, IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContext, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _emailService = emailService;
            _hostingEnvironment = hostingEnvironment;
            _httpContext = httpContext;

            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            _countryLogic = new App_CountryLogic(_cache, new EntityFrameworkGenericRepository<App_CountryPoco>(_dbContext));

            _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
            _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));

            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(_cache, new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            _vehicleTypeScaleLogic = new Lms_VehicleTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_VehicleTypePoco>(_dbContext));


        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeliveryOrderSearchResult(string filteredData)
        {
            ValidateSession();
            List<ViewModel_SearchResult_Order> searchResults = new List<ViewModel_SearchResult_Order>();
            searchResults = GetOrdersByQuery(filteredData);

            return PartialView("_PartialViewDeliveryOrders", searchResults);
        }

        [HttpGet]
        public IActionResult MiscOrderSearchResult(string filteredData)
        {
            ValidateSession();
            List<ViewModel_SearchResult_Order> searchResults = new List<ViewModel_SearchResult_Order>();
            searchResults = GetOrdersByQuery(filteredData);

            return PartialView("_PartialViewMiscOrders", searchResults);
        }

        private List<ViewModel_SearchResult_Order> GetOrdersByQuery(string filteredData) {

            List<ViewModel_SearchResult_Order> searchResults = new List<ViewModel_SearchResult_Order>();

            JObject queryData = new JObject();
            string fromDate = "", toDate = "", optionForWaybill = "", waybillNumber = "", optionForBillTo = "", billToCustomerName = "", optionForShipper = "",
                shipperCustomerName = "", optionForCctn = "", cctnRef = "", optionForActn = "", actnRef = "", optionForConsignee = "",
                consigneeCustomerName = "", optionForDelRef = "", delRef = "", optionForPuRef = "", puRef = "", optionForOrderBy = "",
                orderBy = "", optionForBolRef = "", bolRef = "", optionForProRef = "", proRef = "", optionForCustomerRef = "", customerRef = "";

            if (filteredData != string.Empty)
            {
                queryData = JObject.Parse(filteredData);
                fromDate = queryData.SelectToken("fromDate").ToString();
                toDate = queryData.SelectToken("toDate").ToString();
                optionForWaybill = queryData.SelectToken("optionForWaybill").ToString();
                waybillNumber = queryData.SelectToken("waybillNumber").ToString();
                optionForBillTo = queryData.SelectToken("optionForBillTo").ToString();
                billToCustomerName = queryData.SelectToken("billToCustomerName").ToString();
                optionForShipper = queryData.SelectToken("optionForShipper").ToString();
                shipperCustomerName = queryData.SelectToken("shipperCustomerName").ToString();
                optionForCctn = queryData.SelectToken("optionForCctn").ToString();
                cctnRef = queryData.SelectToken("cctnRef").ToString();

                optionForActn = queryData.SelectToken("optionForActn").ToString();
                actnRef = queryData.SelectToken("actnRef").ToString();
                optionForConsignee = queryData.SelectToken("optionForConsignee").ToString();
                consigneeCustomerName = queryData.SelectToken("consigneeCustomerName").ToString();
                optionForDelRef = queryData.SelectToken("optionForDelRef").ToString();
                delRef = queryData.SelectToken("delRef").ToString();
                optionForPuRef = queryData.SelectToken("optionForPuRef").ToString();
                puRef = queryData.SelectToken("puRef").ToString();
                optionForOrderBy = queryData.SelectToken("optionForOrderBy").ToString();
                orderBy = queryData.SelectToken("orderBy").ToString();
                optionForBolRef = queryData.SelectToken("optionForBolRef").ToString();
                bolRef = queryData.SelectToken("bolRef").ToString();

                optionForProRef = queryData.SelectToken("optionForProRef").ToString();
                proRef = queryData.SelectToken("proRef").ToString();

                optionForCustomerRef = queryData.SelectToken("optionForCustomerRef").ToString();
                customerRef = queryData.SelectToken("customerRef").ToString();
            }

            var orders = _orderLogic.GetList().Where(c => c.OrderTypeId == 1 && (c.TotalOrderCost + c.TotalAdditionalServiceCost) > 0).ToList();

            List<Lms_CustomerPoco> customers = new List<Lms_CustomerPoco>();
            customers = _customerLogic.GetList();

            if (fromDate != "" && toDate != "")
            {
                orders = orders.Where(c => c.ScheduledPickupDate >= Convert.ToDateTime(fromDate) && c.ScheduledPickupDate <= Convert.ToDateTime(toDate)).ToList();
            }

            if (waybillNumber.Trim() != "")
            {
                if (optionForWaybill == "1")
                {
                    orders = orders.Where(c => c.WayBillNumber.Equals(waybillNumber.Trim())).ToList();
                }
                else if (optionForWaybill == "2")
                {
                    orders = orders.Where(c => c.WayBillNumber.Contains(waybillNumber.Trim())).ToList();
                }
            }
            if (customerRef.Trim() != "")
            {
                if (optionForCustomerRef == "1")
                {
                    orders = orders.Where(c => c.ReferenceNumber != null && c.ReferenceNumber.ToUpper().Equals(customerRef.ToUpper())).ToList();
                }
                else if (optionForCustomerRef == "2")
                {
                    orders = orders.Where(c => c.ReferenceNumber != null && c.ReferenceNumber.ToUpper().Contains(customerRef.ToUpper())).ToList();
                }
            }
            if (billToCustomerName.Trim() != "")
            {
                if (optionForBillTo == "1")
                {
                    customers = new List<Lms_CustomerPoco>();
                    customers = _customerLogic.GetList().Where(c => c.CustomerName.Equals(billToCustomerName.ToUpper())).OrderBy(c => c.Id).ToList();
                    orders = orders.Where(c => customers.Any(d => d.Id == c.BillToCustomerId)).ToList();
                }
                else if (optionForBillTo == "2")
                {
                    customers = new List<Lms_CustomerPoco>();
                    customers = _customerLogic.GetList().Where(c => c.CustomerName.ToUpper().Contains(billToCustomerName.ToUpper())).OrderBy(c => c.Id).ToList();
                    orders = orders.Where(c => customers.Any(d => d.Id == c.BillToCustomerId)).ToList();
                }
            }
            if (shipperCustomerName.Trim() != "")
            {
                if (optionForShipper == "1")
                {
                    customers = new List<Lms_CustomerPoco>();
                    customers = _customerLogic.GetList().Where(c => c.CustomerName.ToUpper().Equals(shipperCustomerName.ToUpper())).OrderBy(c => c.Id).ToList();
                    orders = orders.Where(c => customers.Any(d => c.ShipperCustomerId != null && d.Id == c.ShipperCustomerId)).ToList();
                }
                else if (optionForShipper == "2")
                {
                    customers = new List<Lms_CustomerPoco>();
                    customers = _customerLogic.GetList().Where(c => c.CustomerName.ToUpper().Contains(shipperCustomerName.ToUpper())).OrderBy(c => c.Id).ToList();
                    orders = orders.Where(c => customers.Any(d => c.ShipperCustomerId != null && d.Id == c.ShipperCustomerId)).ToList();
                }
            }
            if (consigneeCustomerName.Trim() != "")
            {
                if (optionForConsignee == "1")
                {
                    customers = new List<Lms_CustomerPoco>();
                    customers = _customerLogic.GetList().Where(c => c.CustomerName.ToUpper().Equals(consigneeCustomerName.ToUpper())).OrderBy(c => c.Id).ToList();
                    orders = orders.Where(c => customers.Any(d => c.ConsigneeCustomerId != null && d.Id == c.ConsigneeCustomerId)).ToList();
                }
                else if (optionForConsignee == "2")
                {
                    customers = new List<Lms_CustomerPoco>();
                    customers = _customerLogic.GetList().Where(c => c.CustomerName.ToUpper().Contains(consigneeCustomerName.ToUpper())).OrderBy(c => c.Id).ToList();
                    orders = orders.Where(c => customers.Any(d => c.ConsigneeCustomerId != null && d.Id == c.ConsigneeCustomerId)).ToList();
                }
            }
            if (cctnRef.Trim() != "")
            {
                if (optionForCctn == "1")
                {
                    orders = orders.Where(c => c.CargoCtlNumber != null && c.CargoCtlNumber.ToUpper().Equals(cctnRef.ToUpper())).ToList();
                }
                else if (optionForCctn == "2")
                {
                    orders = orders.Where(c => c.CargoCtlNumber != null && c.CargoCtlNumber.ToUpper().Contains(cctnRef.ToUpper())).ToList();
                }
            }
            if (actnRef.Trim() != "")
            {
                if (optionForActn == "1")
                {
                    orders = orders.Where(c => c.AwbCtnNumber != null && c.AwbCtnNumber.ToUpper().Equals(actnRef.ToUpper())).ToList();
                }
                else if (optionForActn == "2")
                {
                    orders = orders.Where(c => c.AwbCtnNumber != null && c.AwbCtnNumber.ToUpper().Contains(actnRef.ToUpper())).ToList();
                }
            }
            if (delRef.Trim() != "")
            {
                if (optionForDelRef == "1")
                {
                    orders = orders.Where(c => c.DeliveryReferenceNumber != null && c.DeliveryReferenceNumber.ToUpper().Equals(delRef.ToUpper())).ToList();
                }
                else if (optionForDelRef == "2")
                {
                    orders = orders.Where(c => c.DeliveryReferenceNumber != null && c.DeliveryReferenceNumber.ToUpper().Contains(delRef.ToUpper())).ToList();
                }
            }
            if (puRef.Trim() != "")
            {
                if (optionForPuRef == "1")
                {
                    orders = orders.Where(c => c.PickupReferenceNumber != null && c.PickupReferenceNumber.ToUpper().Equals(puRef.ToUpper())).ToList();
                }
                else if (optionForPuRef == "2")
                {
                    orders = orders.Where(c => c.PickupReferenceNumber != null && c.PickupReferenceNumber.ToUpper().Contains(puRef.ToUpper())).ToList();
                }
            }
            if (orderBy.Trim() != "")
            {
                if (orderBy == "1")
                {
                    orders = orders.Where(c => c.OrderedBy != null && c.OrderedBy.ToUpper().Equals(orderBy.ToUpper())).ToList();
                }
                else if (orderBy == "2")
                {
                    orders = orders.Where(c => c.OrderedBy != null && c.OrderedBy.ToUpper().Contains(orderBy.ToUpper())).ToList();
                }
            }
            if (bolRef.Trim() != "")
            {
                if (orderBy == "1")
                {
                    orders = orders.Where(c => c.BolReferenceNumber != null && c.BolReferenceNumber.ToUpper().Equals(bolRef.ToUpper())).ToList();
                }
                else if (orderBy == "2")
                {
                    orders = orders.Where(c => c.BolReferenceNumber != null && c.BolReferenceNumber.ToUpper().Contains(bolRef.ToUpper())).ToList();
                }
            }
            if (proRef.Trim() != "")
            {
                if (orderBy == "1")
                {
                    orders = orders.Where(c => c.ProReferenceNumber != null && c.ProReferenceNumber.ToUpper().Equals(proRef.ToUpper())).ToList();
                }
                else if (orderBy == "2")
                {
                    orders = orders.Where(c => c.ProReferenceNumber != null && c.ProReferenceNumber.ToUpper().Contains(proRef.ToUpper())).ToList();
                }
            }

            customers = _customerLogic.GetList();
            var employees = _employeeLogic.GetList();
            var addresses = _addressLogic.GetList();

            var unitTypes = _unitTypeLogic.GetList();
            var weightScales = _weightScaleLogic.GetList();
            var deliveyOptions = _deliveryOptionLogic.GetList();
            var vehicleTypes = _vehicleTypeScaleLogic.GetList();

            var cities = _cityLogic.GetList();
            var provinces = _provinceLogic.GetList();
            var countries = _countryLogic.GetList();

            foreach (var order in orders)
            {
                var resultModel = new ViewModel_SearchResult_Order();
                resultModel.OrderId = order.Id;
                resultModel.OrderTypeId = order.OrderTypeId;

                resultModel.OrderId = order.OrderTypeId;
                resultModel.OrderTypeId = order.OrderTypeId;
                resultModel.WayBillNumber = order.WayBillNumber;
                resultModel.ReferenceNumber = order.ReferenceNumber;
                resultModel.CargoCtlNumber = order.CargoCtlNumber;
                resultModel.AwbCtnNumber = order.AwbCtnNumber;
                resultModel.PickupReferenceNumber = order.PickupReferenceNumber;
                resultModel.DeliveryReferenceNumber = order.DeliveryReferenceNumber;

                resultModel.ShipperCustomerId = order.ShipperCustomerId;
                if (resultModel.ShipperCustomerId != null && resultModel.ShipperCustomerId > 0)
                {
                    resultModel.ShipperCustomerName = customers.Where(c => c.Id == resultModel.ShipperCustomerId).FirstOrDefault().CustomerName;
                }


                resultModel.ShipperAddressId = order.ShipperAddressId;
                if (resultModel.ShipperAddressId != null && resultModel.ShipperAddressId > 0)
                {
                    var address = addresses.Where(c => c.Id == resultModel.ShipperAddressId).FirstOrDefault();
                    resultModel.ShipperUnitNumber = address.UnitNumber;
                    resultModel.ShipperAddressLine = address.AddressLine;
                    if (address.CityId > 0)
                    {
                        resultModel.ShipperCityName = cities.Where(c => c.Id == address.CityId).FirstOrDefault().CityName;
                    }

                    if (address.ProvinceId > 0)
                    {
                        resultModel.ShipperProvinceName = provinces.Where(c => c.Id == address.ProvinceId).FirstOrDefault().ShortCode;
                    }

                    if (address.CountryId > 0)
                    {
                        resultModel.ShipperCountryName = countries.Where(c => c.Id == address.CountryId).FirstOrDefault().Alpha3CountryCode;
                    }

                    resultModel.ShipperPostCode = address.PostCode;
                }


                resultModel.ConsigneeCustomerId = order.ConsigneeCustomerId;
                if (resultModel.ConsigneeCustomerId != null && resultModel.ConsigneeCustomerId > 0)
                {
                    resultModel.ConsigneeCustomerName = customers.Where(c => c.Id == resultModel.ConsigneeCustomerId).FirstOrDefault().CustomerName;
                }

                resultModel.ConsigneeAddressId = order.ConsigneeAddressId;
                if (resultModel.ConsigneeAddressId != null && resultModel.ConsigneeAddressId > 0)
                {
                    var address = addresses.Where(c => c.Id == resultModel.ConsigneeAddressId).FirstOrDefault();
                    resultModel.ConsigneeUnitNumber = address.UnitNumber;
                    resultModel.ConsigneeAddressLine = address.AddressLine;
                    if (address.CityId > 0)
                    {
                        resultModel.ConsigneeCityName = cities.Where(c => c.Id == address.CityId).FirstOrDefault().CityName;
                    }

                    if (address.ProvinceId > 0)
                    {
                        resultModel.ConsigneeProvinceName = provinces.Where(c => c.Id == address.ProvinceId).FirstOrDefault().ShortCode;
                    }

                    if (address.CountryId > 0)
                    {
                        resultModel.ConsigneeCountryName = countries.Where(c => c.Id == address.CountryId).FirstOrDefault().Alpha3CountryCode;
                    }

                    resultModel.ConsigneePostCode = address.PostCode;
                }

                resultModel.BillToCustomerId = order.BillToCustomerId;
                if (resultModel.BillToCustomerId > 0)
                {
                    resultModel.BillToCustomerName = customers.Where(c => c.Id == resultModel.BillToCustomerId).FirstOrDefault().CustomerName;
                }

                resultModel.ServiceProviderEmployeeId = order.ServiceProviderEmployeeId;
                if (resultModel.ServiceProviderEmployeeId > 0)
                {
                    var emp = employees.Where(c => c.Id == resultModel.ServiceProviderEmployeeId).FirstOrDefault();
                    resultModel.ServiceProviderEmployeeName = emp.FirstName + " " + emp.LastName;
                }


                resultModel.ScheduledPickupDate = order.ScheduledPickupDate;
                resultModel.ExpectedDeliveryDate = order.ExpectedDeliveryDate;
                resultModel.CityId = order.CityId;
                if (resultModel.CityId > 0)
                {
                    resultModel.CityName = cities.Where(c => c.Id == resultModel.CityId).FirstOrDefault().CityName;
                }

                resultModel.DeliveryOptionId = order.DeliveryOptionId;
                if (resultModel.DeliveryOptionId > 0)
                {
                    resultModel.DeliveryOptionName = deliveyOptions.Where(c => c.Id == resultModel.DeliveryOptionId).FirstOrDefault().ShortCode;
                }

                resultModel.VehicleTypeId = order.VehicleTypeId;
                if (resultModel.VehicleTypeId > 0)
                {
                    resultModel.VehicleTypeName = vehicleTypes.Where(c => c.Id == resultModel.VehicleTypeId).FirstOrDefault().TypeName;
                }

                resultModel.UnitTypeId = order.UnitTypeId;
                if (resultModel.UnitTypeId > 0)
                {
                    resultModel.UnitTypeName = unitTypes.Where(c => c.Id == resultModel.UnitTypeId).FirstOrDefault().TypeName;
                }

                resultModel.WeightScaleId = order.WeightScaleId;
                if (resultModel.WeightScaleId > 0)
                {
                    resultModel.WeightScaleName = weightScales.Where(c => c.Id == resultModel.WeightScaleId).FirstOrDefault().ShortCode;
                }

                resultModel.WeightTotal = order.WeightTotal;
                resultModel.UnitQuantity = order.UnitQuantity;
                resultModel.SkidQuantity = order.SkidQuantity;
                resultModel.TotalPiece = order.TotalPiece;

                resultModel.OrderBasicCost = order.OrderBasicCost;
                resultModel.BasicCostOverriden = order.BasicCostOverriden;
                resultModel.FuelSurchargePercentage = order.FuelSurchargePercentage;
                resultModel.DiscountPercentOnOrderCost = order.DiscountPercentOnOrderCost;
                resultModel.ApplicableGstPercent = order.ApplicableGstPercent;
                resultModel.TotalOrderCost = order.TotalOrderCost;
                resultModel.TotalAdditionalServiceCost = order.TotalAdditionalServiceCost;

                resultModel.OrderedBy = order.OrderedBy;
                resultModel.DeliveredBy = order.DeliveredBy;

                resultModel.BolReferenceNumber = order.BolReferenceNumber;
                resultModel.ProReferenceNumber = order.ProReferenceNumber;
                resultModel.ShipperName = order.ShipperName;
                resultModel.ShipperAddress = order.ShipperAddress;
                resultModel.OrderShareAmount = order.OrderShareAmount;
                resultModel.IsSharingOnPercent = order.IsSharingOnPercent;

                resultModel.IsInvoiced = order.IsInvoiced;
                resultModel.CommentsForWayBill = order.CommentsForWayBill;
                resultModel.IsPrintedOnWayBill = order.IsPrintedOnWayBill;
                resultModel.CommentsForInvoice = order.CommentsForInvoice;
                resultModel.IsPrintedOnInvoice = order.IsPrintedOnInvoice;

                resultModel.IsPrePrinted = order.IsPrePrinted;
                resultModel.Remarks = order.Remarks;
                resultModel.OrderDateString = order.CreateDate.ToString("dd-MMM-yyyy");

                searchResults.Add(resultModel);
            }

            return searchResults;


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