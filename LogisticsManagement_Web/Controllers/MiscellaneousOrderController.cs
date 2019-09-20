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
    public class MiscellaneousOrderController : Controller
    {

        //private IMemoryCache _cache;  // To do later 

        private Lms_OrderLogic _orderLogic;
        private Lms_OrderStatusLogic _orderStatusLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_EmployeeLogic _employeeLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private Lms_UnitTypeLogic _unitTypeLogic;
        private Lms_WeightScaleLogic _weightScaleLogic;
        private Lms_OrderAdditionalServiceLogic _orderAdditionalServiceLogic;
        private Lms_AdditionalServiceLogic _additionalServiceLogic;
        private Lms_ConfigurationLogic _configurationLogic;
        private Lms_TariffLogic _tariffLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();
        private readonly IEmailService _emailService;
        private IHostingEnvironment _hostingEnvironment;
        private IHttpContextAccessor _httpContext;

        public MiscellaneousOrderController(IMemoryCache cache, IEmailService emailService, IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContext, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _emailService = emailService;
            _hostingEnvironment = hostingEnvironment;
            _httpContext = httpContext;

            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();
            ViewBag.routingOrderId = TempData["OrderId"];
            ViewBag.isTriggerModify = TempData["TriggerModify"];

            return View(GetMiscellaneousOrders());
        }

        public IActionResult RoutingAction(string id)
        {
            if (id != "")
            {
                TempData["OrderId"] = id;
                TempData["TriggerModify"] = 1;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult LoadMiscellaneousOrders()
        {
            ValidateSession();
            return PartialView("_PartialViewLoadMiscOrders", GetMiscellaneousOrders());
        }

        [HttpPost]
        public IActionResult Add([FromBody]dynamic orderData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (orderData != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        Lms_OrderPoco orderPoco = JsonConvert.DeserializeObject<Lms_OrderPoco>(JsonConvert.SerializeObject(orderData[0]));
                        _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                        var addressList = _addressLogic.GetList();

                        Lms_AddressPoco newAddress = new Lms_AddressPoco();

                        var orderAddressData = (JObject)orderData[0];
                        var customerId = orderAddressData.SelectToken("customerId").ToString();
                        var waybillNumber = orderAddressData.SelectToken("wayBillNumber").ToString();
                        var customerAddressId = orderAddressData.SelectToken("customerAddressId").ToString();
                        var customerAddressline = orderAddressData.SelectToken("customerAddressline").ToString();
                        var customerUnitNo = orderAddressData.SelectToken("customerUnitNo").ToString();
                        var customerCityId = orderAddressData.SelectToken("customerCityId").ToString();
                        var customerProvinceId = orderAddressData.SelectToken("customerProvinceId").ToString();
                        var customerPostcode = orderAddressData.SelectToken("customerPostcode").ToString();
                        var orderDate = orderAddressData.SelectToken("orderDate").ToString();
                        orderPoco.ShipperAddressId = customerAddressId == "" ? 0 : Convert.ToInt32(customerAddressId);
                        orderPoco.ShipperCustomerId = customerId == "" ? 0 : Convert.ToInt32(customerId);
                        orderPoco.IsInvoiced = false;
                        orderPoco.CreateDate = orderDate == "" ? DateTime.Today : Convert.ToDateTime(orderDate);
                        orderPoco.CreatedBy = sessionData.UserId;
                        orderPoco.OrderTypeId = 3;  //3 for misc. order

                        if (orderPoco.OrderShareAmount <= 0)
                        {
                            orderPoco.OrderShareAmount = null;
                            orderPoco.IsSharingOnPercent = null;
                        }

                        var newWbNumber = _orderLogic.GetList().Max(c => c.WayBillNumber);
                        if (!string.IsNullOrEmpty(newWbNumber) && Convert.ToInt32(newWbNumber) > 0)
                        {
                            newWbNumber = (Convert.ToInt32(newWbNumber) + 1).ToString();

                        }
                        else
                        {
                            newWbNumber = _configurationLogic.GetSingleById(1).DeliveryWBNoStartFrom;
                            if (string.IsNullOrEmpty(newWbNumber) && Convert.ToInt32(newWbNumber) < 1)
                            {
                                newWbNumber = "1";
                            }
                        }

                        orderPoco.WayBillNumber = newWbNumber;

                        var customerAddressInfo = addressList.Where(c => c.Id == orderPoco.ShipperAddressId).FirstOrDefault();
                        if (customerAddressInfo != null)
                        {
                            customerAddressInfo.UnitNumber = !string.IsNullOrEmpty(customerAddressInfo.UnitNumber) ? Convert.ToString(customerAddressInfo.UnitNumber).Trim().ToUpper() : "";
                            if (customerAddressline.Trim().ToUpper() == customerAddressInfo.AddressLine.Trim().ToUpper() && customerUnitNo.Trim().ToUpper() == customerAddressInfo.UnitNumber && Convert.ToInt16(customerCityId) == customerAddressInfo.CityId)
                            {
                                if (Convert.ToInt16(customerProvinceId) != customerAddressInfo.ProvinceId || customerPostcode != customerAddressInfo.PostCode)
                                {
                                    customerAddressInfo.ProvinceId = Convert.ToInt16(customerProvinceId);
                                    customerAddressInfo.PostCode = customerPostcode;
                                    _addressLogic.Update(customerAddressInfo);
                                }
                            }
                            else
                            {
                                newAddress = new Lms_AddressPoco();
                                newAddress.AddressLine = customerAddressline;
                                newAddress.UnitNumber = customerUnitNo;
                                newAddress.CityId = Convert.ToInt16(customerCityId);
                                newAddress.ProvinceId = Convert.ToInt16(customerProvinceId);
                                newAddress.CountryId = 41; // default Canada
                                newAddress.PostCode = customerPostcode;
                                newAddress.CreatedBy = sessionData.UserId;
                                orderPoco.ShipperAddressId = _addressLogic.Add(newAddress).Id;
                            }
                        }

                        List<Lms_OrderAdditionalServicePoco> orderAdditionalServices = JsonConvert.DeserializeObject<List<Lms_OrderAdditionalServicePoco>>(JsonConvert.SerializeObject(orderData[1]));
                        _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
                        if (orderPoco.Id < 1 && orderPoco.BillToCustomerId > 0)
                        {
                            var addedOrder = _orderLogic.Add(orderPoco);
                            foreach (var item in orderAdditionalServices)
                            {
                                item.OrderId = addedOrder.Id;
                                var existingRecord = _orderAdditionalServiceLogic.GetList().Where(c => c.OrderId == addedOrder.Id && c.AdditionalServiceId == item.AdditionalServiceId).FirstOrDefault();
                                if (existingRecord == null)
                                {
                                    _orderAdditionalServiceLogic.Add(item);
                                }
                                else
                                {
                                    _orderAdditionalServiceLogic.Remove(existingRecord);
                                    _orderAdditionalServiceLogic.Add(item);
                                }
                            }

                            result = addedOrder.WayBillNumber;
                        }

                        if (result != "")
                        {
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult Update([FromBody]dynamic orderData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (orderData != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        Lms_OrderPoco orderPoco = JsonConvert.DeserializeObject<Lms_OrderPoco>(JsonConvert.SerializeObject(orderData[0]));
                        _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                        var addressList = _addressLogic.GetList();

                        Lms_AddressPoco newAddress = new Lms_AddressPoco();

                        var orderAddressData = (JObject)orderData[0];
                        var customerId = orderAddressData.SelectToken("customerId").ToString();
                        var waybillNumber = orderAddressData.SelectToken("wayBillNumber").ToString();
                        var customerAddressId = orderAddressData.SelectToken("customerAddressId").ToString();
                        var customerAddressline = orderAddressData.SelectToken("customerAddressline").ToString();
                        var customerUnitNo = orderAddressData.SelectToken("customerUnitNo").ToString();
                        var customerCityId = orderAddressData.SelectToken("customerCityId").ToString();
                        var customerProvinceId = orderAddressData.SelectToken("customerProvinceId").ToString();
                        var customerPostcode = orderAddressData.SelectToken("customerPostcode").ToString();
                        var orderDate = orderAddressData.SelectToken("orderDate").ToString();
                        orderPoco.ShipperAddressId = customerAddressId == "" ? 0 : Convert.ToInt32(customerAddressId);
                        orderPoco.ShipperCustomerId = customerId == "" ? 0 : Convert.ToInt32(customerId);
                        orderPoco.CreateDate = orderDate == "" ? DateTime.Today : Convert.ToDateTime(orderDate);

                        if (orderPoco.OrderShareAmount <= 0)
                        {
                            orderPoco.OrderShareAmount = null;
                            orderPoco.IsSharingOnPercent = null;
                        }

                        var customerAddressInfo = addressList.Where(c => c.Id == orderPoco.ShipperAddressId).FirstOrDefault();
                        if (customerAddressInfo != null)
                        {
                            customerAddressInfo.UnitNumber = !string.IsNullOrEmpty(customerAddressInfo.UnitNumber) ? Convert.ToString(customerAddressInfo.UnitNumber).Trim().ToUpper() : "";
                            if (customerAddressline.Trim().ToUpper() == customerAddressInfo.AddressLine.Trim().ToUpper() && customerUnitNo.Trim().ToUpper() == customerAddressInfo.UnitNumber && Convert.ToInt16(customerCityId) == customerAddressInfo.CityId)
                            {
                                if (Convert.ToInt16(customerProvinceId) != customerAddressInfo.ProvinceId || customerPostcode != customerAddressInfo.PostCode)
                                {
                                    customerAddressInfo.ProvinceId = Convert.ToInt16(customerProvinceId);
                                    customerAddressInfo.PostCode = customerPostcode;
                                    _addressLogic.Update(customerAddressInfo);
                                }
                            }
                            else
                            {
                                newAddress = new Lms_AddressPoco();
                                newAddress.AddressLine = customerAddressline;
                                newAddress.UnitNumber = customerUnitNo;
                                newAddress.CityId = Convert.ToInt16(customerCityId);
                                newAddress.ProvinceId = Convert.ToInt16(customerProvinceId);
                                newAddress.CountryId = 41; // default Canada, UI have to change to accept country
                                newAddress.PostCode = customerPostcode;
                                newAddress.CreatedBy = sessionData.UserId;
                                orderPoco.ShipperAddressId = _addressLogic.Add(newAddress).Id;
                            }
                        }

                        List<Lms_OrderAdditionalServicePoco> orderAdditionalServices = JsonConvert.DeserializeObject<List<Lms_OrderAdditionalServicePoco>>(JsonConvert.SerializeObject(orderData[1]));
                        _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
                        if (orderPoco.Id > 0 && orderPoco.BillToCustomerId > 0)
                        {
                            var existingOrder = _orderLogic.GetSingleById(orderPoco.Id);
                            if (existingOrder != null)
                            {
                                existingOrder.CommentsForInvoice = orderPoco.CommentsForInvoice;
                                existingOrder.IsPrintedOnInvoice = orderPoco.IsPrintedOnInvoice;
                                existingOrder.IsPrintedOnWayBill = orderPoco.IsPrintedOnWayBill;
                                existingOrder.CommentsForWayBill = orderPoco.CommentsForWayBill;

                                existingOrder.OrderBasicCost = orderPoco.OrderBasicCost;
                                existingOrder.DiscountPercentOnOrderCost = orderPoco.DiscountPercentOnOrderCost;
                                existingOrder.ApplicableGstPercent = orderPoco.ApplicableGstPercent;
                                existingOrder.TotalOrderCost = orderPoco.TotalOrderCost;
                                existingOrder.TotalAdditionalServiceCost = orderPoco.TotalAdditionalServiceCost;

                                existingOrder.BillToCustomerId = orderPoco.BillToCustomerId;
                                existingOrder.ReferenceNumber = orderPoco.ReferenceNumber;
                                existingOrder.AwbCtnNumber = orderPoco.AwbCtnNumber;
                                existingOrder.CargoCtlNumber = orderPoco.CargoCtlNumber;
                                existingOrder.CreateDate = orderPoco.CreateDate;
                                existingOrder.OrderedBy = orderPoco.OrderedBy;
                                existingOrder.ContactPhoneNumber = orderPoco.ContactPhoneNumber;
                                existingOrder.DepartmentName = orderPoco.DepartmentName;
                                existingOrder.ShipperAddressId = orderPoco.ShipperAddressId;
                                existingOrder.ShipperCustomerId = orderPoco.ShipperCustomerId;
                                existingOrder.ServiceProviderEmployeeId = orderPoco.ServiceProviderEmployeeId;
                                existingOrder.OrderShareAmount = orderPoco.OrderShareAmount;
                                existingOrder.IsSharingOnPercent = orderPoco.IsSharingOnPercent;

                                existingOrder.DeliveredBy = orderPoco.DeliveredBy;
                                existingOrder.BolReferenceNumber = orderPoco.BolReferenceNumber;
                                existingOrder.ProReferenceNumber = orderPoco.ProReferenceNumber;
                                existingOrder.ShipperName = orderPoco.ShipperName;
                                existingOrder.ShipperAddress = orderPoco.ShipperAddress;

                                existingOrder.UnitTypeId = orderPoco.UnitTypeId;
                                existingOrder.UnitQuantity = orderPoco.UnitQuantity;
                                existingOrder.SkidQuantity = orderPoco.SkidQuantity;
                                existingOrder.TotalPiece = orderPoco.TotalPiece;
                                existingOrder.WeightScaleId = orderPoco.WeightScaleId;
                                existingOrder.WeightTotal = orderPoco.WeightTotal;
                                existingOrder.CityId = orderPoco.CityId;

                                var orderServices = _orderAdditionalServiceLogic.GetList().Where(c => c.OrderId == existingOrder.Id).ToList();
                                if (orderServices.Count > 0)
                                {
                                    foreach (var item in orderServices)
                                    {
                                        _orderAdditionalServiceLogic.Remove(item);
                                    }
                                }

                                if (orderAdditionalServices.Count > 0)
                                {
                                    foreach (var item in orderAdditionalServices)
                                    {
                                        if (item.AdditionalServiceId > 0)
                                        {
                                            item.OrderId = existingOrder.Id;
                                            _orderAdditionalServiceLogic.Add(item);
                                        }
                                    }
                                }

                                var updatedOrder = _orderLogic.Update(existingOrder);
                                result = updatedOrder.WayBillNumber;
                            }
                        }

                        if (result != "")
                        {
                            scope.Complete();
                        }
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
            ValidateSession();
            var result = "";
            try
            {
                using (var scope = new TransactionScope())
                {
                    var orders = _orderLogic.GetList().Where(c => c.WayBillNumber == id).ToList();

                    _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
                    _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));

                    foreach (var order in orders)
                    {
                        if (!order.IsInvoiced)
                        {
                            var orderServices = _orderAdditionalServiceLogic.GetList().Where(c => c.OrderId == order.Id).ToList();
                            if (orderServices.Count > 0)
                            {
                                foreach (var item in orderServices)
                                {
                                    _orderAdditionalServiceLogic.Remove(item);
                                }
                            }
                            _orderLogic.Remove(order);
                            result = "Success";
                        }
                        else
                        {
                            result = "";
                        }
                    }

                    if (result.Length > 0)
                    {
                        scope.Complete();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        public JsonResult GetOrderDetailsByWayBillId(string id)
        {
            ValidateSession();

            try
            {
                var orderPocos = _orderLogic.GetList().Where(c => c.WayBillNumber == id).ToList();

                _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
                var orderAdditionalServices = _orderAdditionalServiceLogic.GetList(); //.Where(c => orderPocos.Select(d => d.Id).ToList().Contains(c.OrderId)).ToList();

                orderAdditionalServices = (from addServ in orderAdditionalServices
                                           join order in orderPocos on addServ.OrderId equals order.Id
                                           select addServ).ToList();

                return Json(JsonConvert.SerializeObject(new { orderPocos, orderAdditionalServices }));
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public JsonResult GetOrderInfoByOrderId(string id)
        {
            ValidateSession();

            try
            {
                var orderInfo = _orderLogic.GetList().Where(c => c.Id == Convert.ToInt32(id)).FirstOrDefault();

                return Json(JsonConvert.SerializeObject(orderInfo));
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public JsonResult GetOrderStatusByOrderId(string id)
        {
            ValidateSession();

            try
            {
                var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == Convert.ToInt32(id)).FirstOrDefault();

                return Json(JsonConvert.SerializeObject(orderStatus));
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public JsonResult PrintWaybill([FromBody]dynamic orderData)
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
                            var orderDiscount = Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) - (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) * orderInfo.DiscountPercentOnOrderCost / 100);
                            if (orderDiscount > 0)
                            {
                                waybillPrintViewModel.OrderDiscountAmount = orderDiscount.ToString();
                                waybillPrintViewModel.OrderBasePrice = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) - (decimal)orderDiscount).ToString();
                            }
                            else
                            {
                                waybillPrintViewModel.OrderDiscountAmount = "0.00";
                            }
                        }

                        if (orderInfo.FuelSurchargePercentage != null && orderInfo.FuelSurchargePercentage > 0)
                        {
                            var fuelAmnt = Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) * orderInfo.FuelSurchargePercentage / 100;
                            if (fuelAmnt > 0)
                            {
                                waybillPrintViewModel.FuelSurcharge = fuelAmnt.ToString();
                            }
                            else
                            {
                                waybillPrintViewModel.FuelSurcharge = "0.00";
                            }
                        }

                        if (orderInfo.TotalAdditionalServiceCost > 0)
                        {
                            waybillPrintViewModel.AdditionalServiceCost = orderInfo.TotalAdditionalServiceCost.ToString();
                        }
                        else
                        {
                            waybillPrintViewModel.AdditionalServiceCost = "0.00";
                        }

                        if (orderInfo.ApplicableGstPercent != null && orderInfo.ApplicableGstPercent > 0)
                        {
                            var taxAmnt = Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) * orderInfo.ApplicableGstPercent / 100;
                            if (taxAmnt > 0)
                            {
                                waybillPrintViewModel.OrderTaxAmountOnBasePrice = taxAmnt.ToString();
                            }
                            else
                            {
                                waybillPrintViewModel.OrderTaxAmountOnBasePrice = "0.00";
                            }
                        }

                        waybillPrintViewModel.NetTotalOrderCost = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) + Convert.ToDecimal(waybillPrintViewModel.FuelSurcharge) + Convert.ToDecimal(waybillPrintViewModel.AdditionalServiceCost)).ToString();
                        waybillPrintViewModel.ShipperCustomerName = customers.Where(c => c.Id == orderInfo.ShipperCustomerId).FirstOrDefault().CustomerName;

                        var shippperAddress = addresses.Where(c => c.Id == orderInfo.ShipperAddressId).FirstOrDefault();

                        waybillPrintViewModel.ShipperCustomerAddressLine1 = !string.IsNullOrEmpty(shippperAddress.UnitNumber) ? shippperAddress.UnitNumber + ", " + shippperAddress.AddressLine : shippperAddress.AddressLine;
                        waybillPrintViewModel.ShipperCustomerAddressLine2 = cities.Where(c => c.Id == shippperAddress.CityId).FirstOrDefault().CityName + ", " + provinces.Where(c => c.Id == shippperAddress.ProvinceId).FirstOrDefault().ShortCode + "  " + shippperAddress.PostCode;

                        waybillPrintViewModel.ConsigneeCustomerName = customers.Where(c => c.Id == orderInfo.ConsigneeCustomerId).FirstOrDefault().CustomerName;

                        var consigneeAddress = addresses.Where(c => c.Id == orderInfo.ConsigneeAddressId).FirstOrDefault();


                        waybillPrintViewModel.ConsigneeCustomerAddressLine1 = !string.IsNullOrEmpty(consigneeAddress.UnitNumber) ? consigneeAddress.UnitNumber + ", " + consigneeAddress.AddressLine : consigneeAddress.AddressLine;
                        waybillPrintViewModel.ConsigneeCustomerAddressLine2 = cities.Where(c => c.Id == consigneeAddress.CityId).FirstOrDefault().CityName + ", " + provinces.Where(c => c.Id == consigneeAddress.ProvinceId).FirstOrDefault().ShortCode + "  " + consigneeAddress.PostCode;

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
                var path = "/contents/waybills/waybill_" + uniqueId + ".pdf";
                var filePath = webrootPath + path;

                var pdfReport = new ViewAsPdf("PrintWaybill", waybillPrintViewModels);
                var file = pdfReport.BuildFile(ControllerContext).Result;

                System.IO.File.WriteAllBytes(filePath, file);

                //_emailService.SendEmail("zizaheer@yahoo.com", "test subject", "test body content", path);


                return Json(path);
            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
            //return View();
        }

        private ViewModel_MiscellaneousOrder GetMiscellaneousOrders()
        {
            ViewModel_MiscellaneousOrder miscOrderViewModel = new ViewModel_MiscellaneousOrder();

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            miscOrderViewModel.Cities = _cityLogic.GetList();

            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            miscOrderViewModel.Provinces = _provinceLogic.GetList();

            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
            miscOrderViewModel.Configuration = _configurationLogic.GetList().FirstOrDefault();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            miscOrderViewModel.Customers = _customerLogic.GetList();

            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(_cache, new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            miscOrderViewModel.DeliveryOptions = _deliveryOptionLogic.GetList();

            _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
            miscOrderViewModel.UnitTypes = _unitTypeLogic.GetList();

            _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));
            miscOrderViewModel.WeightScales = _weightScaleLogic.GetList();

            _additionalServiceLogic = new Lms_AdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>(_dbContext));
            miscOrderViewModel.AdditionalServices = _additionalServiceLogic.GetList();

            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            miscOrderViewModel.Employees = _employeeLogic.GetList().Where(c => c.EmployeeTypeId == 5).ToList(); // Load employees 'Broker'

            var orders = _orderLogic.GetList().Where(c => c.OrderTypeId == 3 && c.IsInvoiced == false).ToList(); // Load only misc. orders

            List<ViewModel_OrderDispatched> viewModelOrders = new List<ViewModel_OrderDispatched>();
            foreach (var item in orders)
            {
                ViewModel_OrderDispatched viewModelOrder = new ViewModel_OrderDispatched();
                viewModelOrder.OrderId = item.Id;
                viewModelOrder.OrderTypeId = item.OrderTypeId;
                viewModelOrder.OrderTypeFlag = "Misc.";
                viewModelOrder.ServiceProviderEmployeeId = item.ServiceProviderEmployeeId;
                if (item.ServiceProviderEmployeeId != null && item.ServiceProviderEmployeeId > 0)
                {
                    viewModelOrder.ServiceProviderEmployeeName = (miscOrderViewModel.Employees.Where(c => c.Id == item.ServiceProviderEmployeeId).FirstOrDefault()).FirstName;
                }
                viewModelOrder.WayBillNumber = item.WayBillNumber;
                viewModelOrder.OrderDateString = item.CreateDate.ToString("dd-MMM-yy");

                viewModelOrder.CustomerRefNumber = item.ReferenceNumber;
                viewModelOrder.UnitTypeId = item.UnitTypeId;
                if (item.UnitTypeId > 0)
                {
                    viewModelOrder.UnitTypeName = miscOrderViewModel.UnitTypes.Where(c => c.Id == viewModelOrder.UnitTypeId).FirstOrDefault().ShortCode;
                }
                viewModelOrder.UnitQuantity = item.UnitQuantity;
                viewModelOrder.SkidQuantity = item.SkidQuantity;
                viewModelOrder.TotalPiece = item.TotalPiece;
                viewModelOrder.SpcIns = "";
                viewModelOrder.BillerCustomerId = item.BillToCustomerId;
                if (item.BillToCustomerId > 0)
                {
                    viewModelOrder.BillerCustomerName = miscOrderViewModel.Customers.Where(c => c.Id == viewModelOrder.BillerCustomerId).FirstOrDefault().CustomerName;
                }

                viewModelOrders.Add(viewModelOrder);
            }

            miscOrderViewModel.MiscellaneousOrders = viewModelOrders;

            return miscOrderViewModel;
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