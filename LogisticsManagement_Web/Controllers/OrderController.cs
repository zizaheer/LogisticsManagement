using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
    public class OrderController : Controller
    {

        //private IMemoryCache _cache;  // To do later 

        private Lms_OrderLogic _orderLogic;
        private Lms_OrderStatusLogic _orderStatusLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_CustomerAddressMappingLogic _customerAddressLogic;
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

        private Lms_CompanyInfoLogic _companyInfoLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();
        private readonly IEmailService _emailService;
        private IHostingEnvironment _hostingEnvironment;
        private IHttpContextAccessor _httpContext;

        public OrderController(IMemoryCache cache, IEmailService emailService, IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContext, LogisticsContext dbContext)
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
            return View(GetAllRequiredDataForDispatchBoard());
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
        public IActionResult LoadOrdersForDispatch()
        {
            ValidateSession();
            return PartialView("_PartialViewOrderData", GetAllRequiredDataForDispatchBoard());
        }

        [HttpGet]
        public IActionResult LoadDispatchedOrdersForDispatchBoard()
        {
            ValidateSession();
            return PartialView("_PartialViewDispatchedOrders", GetAllRequiredDataForDispatchBoard());
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
                    Lms_OrderPoco orderPoco = JsonConvert.DeserializeObject<Lms_OrderPoco>(JsonConvert.SerializeObject(orderData[0]));

                    _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                    var addressList = _addressLogic.GetList();

                    Lms_AddressPoco newAddress = new Lms_AddressPoco();

                    var orderAddressData = (JObject)orderData[0];
                    var shipperAddressline = orderAddressData.SelectToken("shipperAddressline").ToString();
                    var shipperUnitNo = orderAddressData.SelectToken("shipperUnitNo").ToString();
                    var shipperCityId = orderAddressData.SelectToken("shipperCityId").ToString();
                    var shipperProvinceId = orderAddressData.SelectToken("shipperProvinceId").ToString();
                    var shipperPostcode = orderAddressData.SelectToken("shipperPostcode").ToString();
                    var shipperCountryId = orderAddressData.SelectToken("shipperCountryId").ToString();

                    newAddress = new Lms_AddressPoco();
                    newAddress.AddressLine = shipperAddressline.Trim().ToUpper();
                    newAddress.UnitNumber = shipperUnitNo.Trim().ToUpper();
                    newAddress.CityId = Convert.ToInt16(shipperCityId);
                    newAddress.ProvinceId = Convert.ToInt16(shipperProvinceId);
                    newAddress.CountryId = Convert.ToInt16(shipperCountryId); // default Canada
                    newAddress.PostCode = shipperPostcode.Trim().ToUpper();
                    newAddress.CreatedBy = sessionData.UserId;


                    var shipperAddressInfo = addressList.Where(c => c.AddressLine == newAddress.AddressLine && c.CityId == newAddress.CityId && c.ProvinceId == newAddress.ProvinceId).FirstOrDefault();
                    if (shipperAddressInfo != null)
                    {
                        if (string.IsNullOrEmpty(newAddress.UnitNumber))
                        {
                            newAddress.UnitNumber = null;
                        }
                        if (string.IsNullOrEmpty(shipperAddressInfo.UnitNumber))
                        {
                            shipperAddressInfo.UnitNumber = null;
                        }

                        if (shipperAddressInfo.UnitNumber != newAddress.UnitNumber)
                        {
                            shipperAddressInfo = null;
                        }
                    }

                    if (shipperAddressInfo != null)
                    {
                        if (newAddress.AddressLine == shipperAddressInfo.AddressLine.Trim().ToUpper() && newAddress.UnitNumber == shipperAddressInfo.UnitNumber && newAddress.CityId == shipperAddressInfo.CityId)
                        {
                            shipperAddressInfo.ProvinceId = newAddress.ProvinceId;
                            shipperAddressInfo.CityId = newAddress.CityId;
                            shipperAddressInfo.PostCode = newAddress.PostCode;
                            orderPoco.ShipperAddressId = _addressLogic.Update(shipperAddressInfo).Id;
                        }
                        else
                        {
                            orderPoco.ShipperAddressId = _addressLogic.Add(newAddress).Id;
                        }
                    }
                    else
                    {
                        orderPoco.ShipperAddressId = _addressLogic.Add(newAddress).Id;
                    }


                    var consigneeAddressline = orderAddressData.SelectToken("consigneeAddressline").ToString();
                    var consigneeUnitNo = orderAddressData.SelectToken("consigneeUnitNo").ToString();
                    var consigneeCityId = orderAddressData.SelectToken("consigneeCityId").ToString();
                    var consigneeProvinceId = orderAddressData.SelectToken("consigneeProvinceId").ToString();
                    var consigneePostcode = orderAddressData.SelectToken("consigneePostcode").ToString();
                    var consigneeCountryId = orderAddressData.SelectToken("consigneeCountryId").ToString();

                    newAddress = new Lms_AddressPoco();
                    newAddress.AddressLine = consigneeAddressline.Trim().ToUpper();
                    newAddress.UnitNumber = consigneeUnitNo.Trim().ToUpper();
                    newAddress.CityId = Convert.ToInt16(consigneeCityId);
                    newAddress.ProvinceId = Convert.ToInt16(consigneeProvinceId);
                    newAddress.CountryId = Convert.ToInt16(consigneeCountryId); // default Canada
                    newAddress.PostCode = consigneePostcode.Trim().ToUpper();
                    newAddress.CreatedBy = sessionData.UserId;

                    var consigneeAddressInfo = addressList.Where(c => c.AddressLine == newAddress.AddressLine && c.CityId == newAddress.CityId && c.ProvinceId == newAddress.ProvinceId).FirstOrDefault();
                    if (consigneeAddressInfo != null)
                    {
                        if (string.IsNullOrEmpty(newAddress.UnitNumber))
                        {
                            newAddress.UnitNumber = null;
                        }
                        if (string.IsNullOrEmpty(consigneeAddressInfo.UnitNumber))
                        {
                            consigneeAddressInfo.UnitNumber = null;
                        }

                        if (consigneeAddressInfo.UnitNumber != newAddress.UnitNumber)
                        {
                            consigneeAddressInfo = null;
                        }
                    }

                    if (consigneeAddressInfo != null)
                    {
                        if (newAddress.AddressLine == consigneeAddressInfo.AddressLine.Trim().ToUpper() && newAddress.UnitNumber == consigneeAddressInfo.UnitNumber && newAddress.CityId == consigneeAddressInfo.CityId)
                        {
                            consigneeAddressInfo.ProvinceId = newAddress.ProvinceId;
                            consigneeAddressInfo.CityId = newAddress.CityId;
                            consigneeAddressInfo.PostCode = newAddress.PostCode;
                            orderPoco.ConsigneeAddressId = _addressLogic.Update(consigneeAddressInfo).Id;
                        }
                        else
                        {
                            orderPoco.ConsigneeAddressId = _addressLogic.Add(newAddress).Id;
                        }
                    }
                    else
                    {
                        orderPoco.ConsigneeAddressId = _addressLogic.Add(newAddress).Id;
                    }

                    List<Lms_OrderAdditionalServicePoco> orderAdditionalServices = JsonConvert.DeserializeObject<List<Lms_OrderAdditionalServicePoco>>(JsonConvert.SerializeObject(orderData[1]));

                    if (orderPoco.Id < 1 && orderPoco.BillToCustomerId > 0 && orderPoco.ShipperCustomerId > 0 && orderPoco.ConsigneeCustomerId > 0)
                    {
                        orderPoco.CreatedBy = sessionData.UserId;
                        var orderInfo = CreateDeliveryOrder(orderPoco, orderAdditionalServices);
                        if (!string.IsNullOrEmpty(orderInfo))
                        {
                            result = orderInfo;
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
                    Lms_OrderPoco orderPoco = JsonConvert.DeserializeObject<Lms_OrderPoco>(JsonConvert.SerializeObject(orderData[0]));
                    List<Lms_OrderAdditionalServicePoco> orderAdditionalServices = JsonConvert.DeserializeObject<List<Lms_OrderAdditionalServicePoco>>(JsonConvert.SerializeObject(orderData[1]));

                    _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                    var addressList = _addressLogic.GetList();

                    Lms_AddressPoco newAddress = new Lms_AddressPoco();

                    var orderAddressData = (JObject)orderData[0];
                    var shipperAddressline = orderAddressData.SelectToken("shipperAddressline").ToString();
                    var shipperUnitNo = orderAddressData.SelectToken("shipperUnitNo").ToString();
                    var shipperCityId = orderAddressData.SelectToken("shipperCityId").ToString();
                    var shipperProvinceId = orderAddressData.SelectToken("shipperProvinceId").ToString();
                    var shipperPostcode = orderAddressData.SelectToken("shipperPostcode").ToString();
                    var shipperCountryId = orderAddressData.SelectToken("shipperCountryId").ToString();

                    newAddress = new Lms_AddressPoco();
                    newAddress.AddressLine = shipperAddressline.Trim().ToUpper();
                    newAddress.UnitNumber = shipperUnitNo.Trim().ToUpper();
                    newAddress.CityId = Convert.ToInt16(shipperCityId);
                    newAddress.ProvinceId = Convert.ToInt16(shipperProvinceId);
                    newAddress.CountryId = Convert.ToInt16(shipperCountryId); // default Canada
                    newAddress.PostCode = shipperPostcode.Trim().ToUpper();
                    newAddress.CreatedBy = sessionData.UserId;

                    //var shipperAddressInfo = addressList.Where(c => c.Id == orderPoco.ShipperAddressId).FirstOrDefault();
                    var shipperAddressInfo = addressList.Where(c => c.AddressLine == newAddress.AddressLine && c.CityId == newAddress.CityId && c.ProvinceId == newAddress.ProvinceId).FirstOrDefault();
                    if (shipperAddressInfo != null)
                    {
                        if (string.IsNullOrEmpty(newAddress.UnitNumber))
                        {
                            newAddress.UnitNumber = null;
                        }
                        if (string.IsNullOrEmpty(shipperAddressInfo.UnitNumber))
                        {
                            shipperAddressInfo.UnitNumber = null;
                        }

                        if (shipperAddressInfo.UnitNumber != newAddress.UnitNumber)
                        {
                            shipperAddressInfo = null;
                        }
                    }

                    if (shipperAddressInfo != null)
                    {
                        if (newAddress.AddressLine == shipperAddressInfo.AddressLine.Trim().ToUpper() && newAddress.UnitNumber == shipperAddressInfo.UnitNumber && newAddress.CityId == shipperAddressInfo.CityId)
                        {
                            shipperAddressInfo.ProvinceId = newAddress.ProvinceId;
                            shipperAddressInfo.CityId = newAddress.CityId;
                            shipperAddressInfo.PostCode = newAddress.PostCode;
                            orderPoco.ShipperAddressId = _addressLogic.Update(shipperAddressInfo).Id;
                        }
                        else
                        {
                            orderPoco.ShipperAddressId = _addressLogic.Add(newAddress).Id;
                        }
                    }
                    else
                    {
                        orderPoco.ShipperAddressId = _addressLogic.Add(newAddress).Id;
                    }

                    var consigneeAddressline = orderAddressData.SelectToken("consigneeAddressline").ToString();
                    var consigneeUnitNo = orderAddressData.SelectToken("consigneeUnitNo").ToString();
                    var consigneeCityId = orderAddressData.SelectToken("consigneeCityId").ToString();
                    var consigneeProvinceId = orderAddressData.SelectToken("consigneeProvinceId").ToString();
                    var consigneePostcode = orderAddressData.SelectToken("consigneePostcode").ToString();
                    var consigneeCountryId = orderAddressData.SelectToken("consigneeCountryId").ToString();

                    newAddress = new Lms_AddressPoco();
                    newAddress.AddressLine = consigneeAddressline.Trim().ToUpper();
                    newAddress.UnitNumber = consigneeUnitNo.Trim().ToUpper();
                    newAddress.CityId = Convert.ToInt16(consigneeCityId);
                    newAddress.ProvinceId = Convert.ToInt16(consigneeProvinceId);
                    newAddress.CountryId = Convert.ToInt16(consigneeCountryId); // default Canada
                    newAddress.PostCode = consigneePostcode.Trim().ToUpper();
                    newAddress.CreatedBy = sessionData.UserId;

                    var consigneeAddressInfo = addressList.Where(c => c.Id == orderPoco.ConsigneeAddressId).FirstOrDefault();
                    if (consigneeAddressInfo != null)
                    {
                        consigneeAddressInfo.UnitNumber = !string.IsNullOrEmpty(consigneeAddressInfo.UnitNumber) ? consigneeAddressInfo.UnitNumber.ToUpper() : "";
                        if (newAddress.AddressLine == consigneeAddressInfo.AddressLine.Trim().ToUpper() && newAddress.UnitNumber == consigneeAddressInfo.UnitNumber && newAddress.CityId == consigneeAddressInfo.CityId)
                        {
                            consigneeAddressInfo.ProvinceId = newAddress.ProvinceId;
                            consigneeAddressInfo.CityId = newAddress.CityId;
                            consigneeAddressInfo.PostCode = newAddress.PostCode;
                            orderPoco.ConsigneeAddressId = _addressLogic.Update(consigneeAddressInfo).Id;
                        }
                        else
                        {
                            orderPoco.ConsigneeAddressId = _addressLogic.Add(newAddress).Id;
                        }
                    }
                    else
                    {
                        orderPoco.ConsigneeAddressId = _addressLogic.Add(newAddress).Id;
                    }



                    var existingOrder = new Lms_OrderPoco();

                    if (orderPoco != null && orderPoco.OrderTypeId > 0)
                    {
                        existingOrder = _orderLogic.GetSingleById(orderPoco.Id);// GetList().Where(c => c.OrderTypeId == orderPoco.OrderTypeId && c.WayBillNumber == orderPoco.WayBillNumber).FirstOrDefault();
                    }

                    if (existingOrder != null && existingOrder.Id > 0)
                    {
                        existingOrder.ReferenceNumber = orderPoco.ReferenceNumber;
                        existingOrder.WayBillNumber = orderPoco.WayBillNumber;
                        existingOrder.CargoCtlNumber = orderPoco.CargoCtlNumber;
                        existingOrder.AwbCtnNumber = orderPoco.AwbCtnNumber;
                        existingOrder.PickupReferenceNumber = orderPoco.PickupReferenceNumber;
                        existingOrder.DeliveryReferenceNumber = orderPoco.DeliveryReferenceNumber;
                        existingOrder.ShipperCustomerId = orderPoco.ShipperCustomerId;
                        existingOrder.ShipperAddressId = orderPoco.ShipperAddressId;
                        existingOrder.ConsigneeCustomerId = orderPoco.ConsigneeCustomerId;
                        existingOrder.ConsigneeAddressId = orderPoco.ConsigneeAddressId;
                        existingOrder.BillToCustomerId = orderPoco.BillToCustomerId;
                        existingOrder.ScheduledPickupDate = orderPoco.ScheduledPickupDate;

                        existingOrder.CityId = orderPoco.CityId;
                        existingOrder.DeliveryOptionId = orderPoco.DeliveryOptionId;
                        existingOrder.VehicleTypeId = orderPoco.VehicleTypeId;
                        existingOrder.WeightScaleId = orderPoco.WeightScaleId;
                        existingOrder.WeightTotal = orderPoco.WeightTotal;
                        if (orderPoco.UnitQuantity != null && orderPoco.UnitQuantity > 0)
                        {
                            existingOrder.UnitTypeId = orderPoco.UnitTypeId;
                            existingOrder.UnitQuantity = orderPoco.UnitQuantity;
                        }
                        else
                        {
                            existingOrder.UnitTypeId = 0;
                            existingOrder.UnitQuantity = null;
                        }
                        existingOrder.SkidQuantity = orderPoco.SkidQuantity;
                        existingOrder.OrderBasicCost = orderPoco.OrderBasicCost;
                        existingOrder.BasicCostOverriden = orderPoco.BasicCostOverriden;
                        existingOrder.FuelSurchargePercentage = orderPoco.FuelSurchargePercentage;
                        existingOrder.DiscountPercentOnOrderCost = orderPoco.DiscountPercentOnOrderCost;
                        existingOrder.ApplicableGstPercent = orderPoco.ApplicableGstPercent;
                        existingOrder.TotalOrderCost = orderPoco.TotalOrderCost;
                        existingOrder.TotalAdditionalServiceCost = orderPoco.TotalAdditionalServiceCost;
                        existingOrder.OrderedBy = orderPoco.OrderedBy;
                        existingOrder.ContactName = orderPoco.ContactName;
                        existingOrder.ContactPhoneNumber = orderPoco.ContactPhoneNumber;
                        existingOrder.Remarks = orderPoco.Remarks;
                        existingOrder.IsPrintedOnWayBill = orderPoco.IsPrintedOnWayBill;
                        existingOrder.CommentsForWayBill = orderPoco.CommentsForWayBill;
                        existingOrder.IsPrintedOnInvoice = orderPoco.IsPrintedOnInvoice;
                        existingOrder.CommentsForInvoice = orderPoco.CommentsForInvoice;


                        _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));

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

                        var poco = _orderLogic.Update(existingOrder);

                        result = poco.Id.ToString();

                    }
                    else if (orderPoco.OrderTypeId == 2)
                    {
                        orderPoco.CreatedBy = sessionData.UserId;
                        var orderInfo = CreateDeliveryOrder(orderPoco, orderAdditionalServices);
                        if (!string.IsNullOrEmpty(orderInfo))
                        {
                            result = orderInfo;
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
                        var orderServices = _orderAdditionalServiceLogic.GetList().Where(c => c.OrderId == order.Id).ToList();
                        if (orderServices.Count > 0)
                        {
                            foreach (var item in orderServices)
                            {
                                _orderAdditionalServiceLogic.Remove(item);
                            }
                        }

                        var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == order.Id).ToList();

                        if (orderStatus.Count > 0)
                        {
                            foreach (var item in orderStatus)
                            {
                                _orderStatusLogic.Remove(item);
                            }
                        }

                        _orderLogic.Remove(order);
                    }

                    scope.Complete();

                    result = "Success";
                }

            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        [HttpPost]
        public IActionResult RemoveByOrderId(string id)
        {
            ValidateSession();
            var result = "";
            try
            {
                using (var scope = new TransactionScope())
                {

                    var order = _orderLogic.GetSingleById(Convert.ToInt32(id));

                    _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
                    _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));

                    var orderServices = _orderAdditionalServiceLogic.GetList().Where(c => c.OrderId == order.Id).ToList();
                    if (orderServices.Count > 0)
                    {
                        foreach (var item in orderServices)
                        {
                            _orderAdditionalServiceLogic.Remove(item);
                        }
                    }

                    var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == order.Id).ToList();

                    if (orderStatus.Count > 0)
                    {
                        foreach (var item in orderStatus)
                        {
                            _orderStatusLogic.Remove(item);
                        }
                    }

                    _orderLogic.Remove(order);


                    scope.Complete();

                    result = "Success";
                }

            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult UpdateDispatchStatus([FromBody]dynamic orderData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (orderData != null)
                {
                    var wayBillNumberList = JArray.Parse(JsonConvert.SerializeObject(orderData[0]));
                    var employeeNumber = Convert.ToInt32(orderData[1]);
                    var dispatchDate = Convert.ToDateTime(orderData[2]);
                    var vehicleId = Convert.ToInt16(orderData[3]);
                    var isShareOnPercent = Convert.ToBoolean(orderData[4]);
                    var shareAmount = Convert.ToDecimal(orderData[5]);
                    var isSendEmail = Convert.ToBoolean(orderData[6]);
                    var emailAddress = Convert.ToString(orderData[7]);

                    string waybillNumbersForEmail = "";

                    var orders = _orderLogic.GetList();
                    var orderStatuses = _orderStatusLogic.GetList();

                    using (var scope = new TransactionScope())
                    {
                        foreach (var item in wayBillNumberList)
                        {
                            var wbNumber = item.ToString();
                            if (waybillNumbersForEmail != "")
                            {
                                waybillNumbersForEmail = waybillNumbersForEmail + ",";
                            }
                            waybillNumbersForEmail = waybillNumbersForEmail + wbNumber;

                            var filteredOrders = orders.Where(c => c.WayBillNumber == wbNumber).ToList();
                            foreach (var order in filteredOrders)
                            {
                                var status = orderStatuses.Where(c => c.OrderId == order.Id).FirstOrDefault();

                                status.IsDispatched = true;
                                status.DispatchedToEmployeeId = employeeNumber;
                                status.VehicleId = vehicleId;
                                status.DispatchedDatetime = order.ScheduledPickupDate; //changed due to customer requested // dispatchDate == null ? DateTime.Now : dispatchDate;
                                status.StatusLastUpdatedOn = DateTime.Now;

                                _orderStatusLogic.Update(status);

                                if (shareAmount > 0)
                                {
                                    order.OrderShareAmount = shareAmount;
                                    order.IsSharingOnPercent = isShareOnPercent;
                                    _orderLogic.Update(order);
                                }
                            }
                        }

                        if (isSendEmail == true && !string.IsNullOrEmpty(emailAddress))
                        {
                            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));

                            string firstName = _employeeLogic.GetSingleById((int)employeeNumber).FirstName;
                            SendDispatchEmail(firstName, emailAddress, waybillNumbersForEmail);
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
        public IActionResult UpdatePickupStatus([FromBody]dynamic orderData)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (orderData != null)
                {
                    var waitTime = string.IsNullOrEmpty(Convert.ToString(orderData[1])) == true ? null : Convert.ToDecimal(orderData[1]);
                    var pickupDate = Convert.ToDateTime(orderData[2]);
                    var orderId = Convert.ToInt16(orderData[3]);

                    var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == orderId).FirstOrDefault();
                    if (orderStatus != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(pickupDate)))
                            {
                                orderStatus.IsPickedup = true;
                            }

                            orderStatus.PickupWaitTimeHour = waitTime;
                            orderStatus.PickupDatetime = pickupDate;
                            orderStatus.StatusLastUpdatedOn = DateTime.Now;

                            _orderStatusLogic.Update(orderStatus);
                            scope.Complete();

                            result = "Success";
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
        public IActionResult UpdatePassonStatus([FromBody]dynamic orderData)
        {

            ValidateSession();
            var result = "";

            try
            {
                if (orderData != null)
                {
                    var wayBillNumber = Convert.ToString(orderData[0]);
                    var waitTime = string.IsNullOrEmpty(Convert.ToString(orderData[1])) == true ? null : Convert.ToDecimal(orderData[1]);
                    var passOnToEmployeeId = string.IsNullOrEmpty(Convert.ToString(orderData[2])) == true ? null : Convert.ToInt16(orderData[2]);
                    var passOffDate = Convert.ToDateTime(orderData[3]);
                    var orderId = Convert.ToInt16(orderData[4]);

                    var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == orderId).FirstOrDefault();

                    if (orderStatus != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(passOffDate)))
                            {
                                orderStatus.IsPassedOff = true;
                            }

                            orderStatus.PassOffWaitTimeHour = waitTime;
                            orderStatus.PassOffDatetime = passOffDate;
                            orderStatus.PassedOffFromEmployeeId = orderStatus.DispatchedToEmployeeId;
                            if (orderStatus.PassedOffToEmployeeId != null)
                            {
                                orderStatus.PassedOffFromEmployeeId = orderStatus.PassedOffToEmployeeId;
                            }
                            orderStatus.PassedOffToEmployeeId = passOnToEmployeeId;
                            orderStatus.StatusLastUpdatedOn = DateTime.Now;

                            _orderStatusLogic.Update(orderStatus);
                            scope.Complete();

                            result = "Success";
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
        public IActionResult UpdateDeliveryStatus([FromBody]dynamic orderData)
        {

            ValidateSession();
            var result = "";

            try
            {
                if (orderData != null)
                {
                    var wayBillNumber = Convert.ToString(orderData[0]);
                    var waitTime = string.IsNullOrEmpty(Convert.ToString(orderData[1])) == true ? null : Convert.ToDecimal(orderData[1]);
                    var deliveryDate = Convert.ToDateTime(orderData[2]);
                    var proofNote = Convert.ToString(orderData[3]);
                    var receivedByName = Convert.ToString(orderData[4]);
                    var receivedBySign = Convert.ToString(orderData[5]);
                    var orderId = Convert.ToInt16(orderData[6]);

                    byte[] imageByte = null;
                    if (receivedBySign != null && receivedBySign != "")
                    {
                        var base64Signature = receivedBySign.Split(",")[1];
                        imageByte = string.IsNullOrEmpty(base64Signature) == true ? null : Convert.FromBase64String(base64Signature);
                    }

                    var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == orderId).FirstOrDefault();

                    if (orderStatus != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(deliveryDate)))
                            {
                                orderStatus.IsDelivered = true;
                            }

                            orderStatus.DeliveredDatetime = deliveryDate;
                            orderStatus.DeliveryWaitTimeHour = waitTime;
                            orderStatus.ReceivedByName = receivedByName;
                            orderStatus.ReceivedBySignature = imageByte;
                            orderStatus.ProofOfDeliveryNote = proofNote;
                            orderStatus.StatusLastUpdatedOn = DateTime.Now;

                            _orderStatusLogic.Update(orderStatus);
                            scope.Complete();

                            result = "Success";
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
        public IActionResult RemoveDispatchStatus(string id)
        {
            var result = "";
            try
            {
                var orders = _orderLogic.GetList().Where(c => c.WayBillNumber == id).ToList();
                var dispatchedList = _orderStatusLogic.GetList();

                dispatchedList = (from dispatch in dispatchedList
                                  join order in orders on dispatch.OrderId equals order.Id
                                  select dispatch).ToList();

                using (var scope = new TransactionScope())
                {

                    foreach (var order in orders)
                    {
                        if (order.OrderShareAmount != null)
                        {
                            order.OrderShareAmount = null;
                            order.IsSharingOnPercent = null;
                            _orderLogic.Update(order);
                        }
                    }

                    foreach (var orderStatus in dispatchedList)
                    {
                        orderStatus.IsDispatched = null;
                        orderStatus.DispatchedDatetime = null;
                        orderStatus.DispatchedToEmployeeId = null;

                        orderStatus.IsPickedup = null;
                        orderStatus.PickupWaitTimeHour = null;
                        orderStatus.PickupDatetime = null;

                        orderStatus.IsPassedOff = null;
                        orderStatus.PassOffWaitTimeHour = null;
                        orderStatus.PassOffDatetime = null;
                        orderStatus.PassedOffToEmployeeId = null;

                        orderStatus.IsDelivered = null;
                        orderStatus.DeliveredDatetime = null;
                        orderStatus.DeliveryWaitTimeHour = null;
                        orderStatus.ReceivedByName = null;
                        orderStatus.ReceivedBySignature = null;
                        orderStatus.ProofOfDeliveryNote = null;
                        orderStatus.StatusLastUpdatedOn = DateTime.Now;

                        _orderStatusLogic.Update(orderStatus);
                    }

                    scope.Complete();

                    result = "Success";
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult RemovePickupStatus(string id)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (id != null)
                {
                    var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == Convert.ToInt32(id)).FirstOrDefault();
                    if (orderStatus != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            orderStatus.IsPickedup = null;
                            orderStatus.PickupWaitTimeHour = null;
                            orderStatus.PickupDatetime = null;
                            orderStatus.IsDelivered = null;
                            orderStatus.DeliveredDatetime = null;
                            orderStatus.DeliveryWaitTimeHour = null;
                            orderStatus.ReceivedByName = null;
                            orderStatus.ReceivedBySignature = null;
                            orderStatus.ProofOfDeliveryNote = null;
                            orderStatus.IsPassedOff = null;
                            orderStatus.PassOffWaitTimeHour = null;
                            orderStatus.PassOffDatetime = null;
                            orderStatus.PassedOffToEmployeeId = null;

                            orderStatus.StatusLastUpdatedOn = DateTime.Now;

                            _orderStatusLogic.Update(orderStatus);
                            scope.Complete();

                            result = "Success";
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
        public IActionResult RemovePassonStatus(string id)
        {

            ValidateSession();
            var result = "";

            try
            {
                if (id != null)
                {
                    var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == Convert.ToInt32(id)).FirstOrDefault();

                    if (orderStatus != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            orderStatus.IsPassedOff = null;
                            orderStatus.PassOffWaitTimeHour = null;
                            orderStatus.PassOffDatetime = null;
                            orderStatus.PassedOffToEmployeeId = null;
                            orderStatus.StatusLastUpdatedOn = DateTime.Now;

                            _orderStatusLogic.Update(orderStatus);
                            scope.Complete();

                            result = "Success";
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
        public IActionResult RemoveDeliveryStatus(string id)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (id != null)
                {
                    var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == Convert.ToInt32(id)).FirstOrDefault();
                    if (orderStatus != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            orderStatus.IsDelivered = null;
                            orderStatus.DeliveredDatetime = null;
                            orderStatus.DeliveryWaitTimeHour = null;
                            orderStatus.ReceivedByName = null;
                            orderStatus.ReceivedBySignature = null;
                            orderStatus.ProofOfDeliveryNote = null;
                            orderStatus.StatusLastUpdatedOn = DateTime.Now;

                            _orderStatusLogic.Update(orderStatus);
                            scope.Complete();

                            result = "Success";
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
        public IActionResult RemoveDeliveryStatusByWaybill(string id)
        {
            ValidateSession();
            var result = "";

            try
            {
                if (id != null)
                {
                    var orders = _orderLogic.GetList().Where(c => c.WayBillNumber == id).ToList();
                    foreach (var order in orders)
                    {
                        var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == order.Id).FirstOrDefault();
                        if (orderStatus != null)
                        {
                            using (var scope = new TransactionScope())
                            {
                                orderStatus.IsDelivered = null;
                                orderStatus.DeliveredDatetime = null;
                                orderStatus.DeliveryWaitTimeHour = null;
                                orderStatus.ReceivedByName = null;
                                orderStatus.ReceivedBySignature = null;
                                orderStatus.ProofOfDeliveryNote = null;
                                orderStatus.StatusLastUpdatedOn = DateTime.Now;

                                _orderStatusLogic.Update(orderStatus);
                                scope.Complete();

                                result = "Success";
                            }
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
        public IActionResult AddPrePrintedWaybill(string fromNumber, string toNumber)
        {
            var result = "";
            try
            {
                int wbTo = 0;
                wbTo = Convert.ToInt32(toNumber);

                int wbFrom = 0;
                wbFrom = Convert.ToInt32(fromNumber);

                List<ViewModel_PrintWaybill> waybillPrintViewModels = new List<ViewModel_PrintWaybill>();

                if (wbTo > 0 && wbFrom > 0)
                {
                    var orders = _orderLogic.GetList();
                    for (int index = wbFrom; index <= wbTo; index++)
                    {
                        var orderByWaybill = orders.Where(c => c.WayBillNumber == index.ToString()).FirstOrDefault();
                        if (orderByWaybill != null && !string.IsNullOrEmpty(orderByWaybill.WayBillNumber) && orderByWaybill.OrderTypeId == 1 && (orderByWaybill.TotalOrderCost + orderByWaybill.TotalAdditionalServiceCost) > 0)
                        {
                            continue;
                        }
                        using (var scope = new TransactionScope())
                        {
                            ViewModel_PrintWaybill waybillPrintViewModel = new ViewModel_PrintWaybill();
                            waybillPrintViewModel.WaybillNumber = index.ToString();
                            waybillPrintViewModel.OrderDiscountAmount = "";
                            waybillPrintViewModel.NumberOfCopyOnEachPage = 2;
                            waybillPrintViewModel.NumberOfCopyPerItem = 2;

                            Lms_OrderPoco order = new Lms_OrderPoco();
                            order.OrderTypeId = 1;
                            order.WayBillNumber = index.ToString();
                            order.ReferenceNumber = "";
                            order.CargoCtlNumber = "";
                            order.AwbCtnNumber = "";
                            order.ShipperCustomerId = 0;
                            order.ShipperAddressId = 0;
                            order.DeliveryOptionId = 1;

                            order.ConsigneeCustomerId = 0;
                            order.ConsigneeAddressId = 0;

                            order.BillToCustomerId = 0;
                            order.ServiceProviderEmployeeId = null;
                            order.UnitTypeId = 0;
                            order.OrderBasicCost = 0;
                            order.TotalAdditionalServiceCost = 0;
                            order.TotalOrderCost = 0;
                            order.IsInvoiced = false;
                            order.IsPrePrinted = true;
                            order.CreateDate = DateTime.Now;
                            order.CreatedBy = sessionData.UserId;

                            var orderInfo = _orderLogic.Add(order);

                            Lms_OrderStatusPoco orderStatusPoco = new Lms_OrderStatusPoco();
                            var trackingNumber = "";
                            string sequecNo = DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString() + orderInfo.Id.ToString();
                            trackingNumber = "TRK01-" + sequecNo;

                            orderStatusPoco.OrderId = orderInfo.Id;
                            orderStatusPoco.TrackingNumber = trackingNumber;
                            orderStatusPoco.StatusLastUpdatedOn = DateTime.Now;
                            orderStatusPoco.CreateDate = DateTime.Now;

                            _orderStatusLogic.Add(orderStatusPoco);

                            waybillPrintViewModels.Add(waybillPrintViewModel);


                            scope.Complete();
                        }
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

                    var webrootPath = _hostingEnvironment.WebRootPath;
                    var uniqueId = DateTime.Now.ToFileTime();
                    var fileName = "waybill_" + uniqueId + ".pdf";
                    var directoryPath = webrootPath + "/contents/waybills/";
                    var filePath = directoryPath + fileName;

                    if (!System.IO.Directory.Exists(directoryPath))
                    {
                        System.IO.Directory.CreateDirectory(directoryPath);
                    }

                    var pdfReport = new ViewAsPdf("PrintDeliveryWaybill", waybillPrintViewModels)
                    {
                        PageSize = Rotativa.AspNetCore.Options.Size.Letter
                    };
                    var file = pdfReport.BuildFile(ControllerContext).Result;

                    System.IO.File.WriteAllBytes(filePath, file);

                    string returnPath = "/contents/waybills/" + fileName;


                    result = returnPath;
                }
            }
            catch (Exception ex)
            {
                //
            }

            return Json(result);
        }

        //public JsonResult FindDuplicateWayBillByWayBillNumber(string id)
        //{
        //    ValidateSession();
        //    string result = "";
        //    try
        //    {
        //        var orderPoco = _orderLogic.GetList().Where(c => c.WayBillNumber == id).FirstOrDefault();
        //        if (orderPoco != null)
        //        {
        //            result = orderPoco.WayBillNumber;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return Json(result);
        //}

        public JsonResult FindDuplicateWayBillByOrderAndWaybillId(string orderId, string orderTypeId, string waybillNo)
        {
            ValidateSession();
            string result = "";
            try
            {
                var existingOrderId = 0;
                var suppliedOrderId = Convert.ToInt32(orderId);

                //this is to filter out all orders excluding the preprinted waybills
                var orderPocoList = _orderLogic.GetList().Where(c => c.WayBillNumber == waybillNo && c.OrderTypeId == Convert.ToInt32(orderTypeId) && c.BillToCustomerId > 0 && (c.TotalOrderCost + c.TotalAdditionalServiceCost) > 0).ToList();
                if (orderPocoList.Count > 0)
                {
                    existingOrderId = orderPocoList.FirstOrDefault().Id;
                }
                //if (orderPocoList.Count > 1)
                //{
                //    result = orderPocoList.FirstOrDefault().WayBillNumber;
                //}

                if (suppliedOrderId > 0)
                {
                    if (existingOrderId > 0 && suppliedOrderId != existingOrderId)
                    {
                        result = "Exists";
                    }
                }
                else
                {
                    if (existingOrderId > 0)
                    {
                        result = "Exists";
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return Json(result);
        }

        public JsonResult IsPrePrintedWaybillForNewEntry(string orderTypeId, string waybillNo)
        {
            ValidateSession();
            string result = "";
            try
            {
                var orderPoco = _orderLogic.GetList().Where(c => c.WayBillNumber == waybillNo && c.OrderTypeId == Convert.ToInt32(orderTypeId) && c.BillToCustomerId <= 0 && (c.TotalOrderCost + c.TotalAdditionalServiceCost) <= 0 && c.IsPrePrinted == true).FirstOrDefault();
                if (orderPoco != null && !string.IsNullOrEmpty(orderPoco.WayBillNumber))
                {
                    result = orderPoco.Id.ToString();
                }
            }
            catch (Exception ex)
            {
            }

            return Json(result);
        }

        public JsonResult GetNextWaybillNumber(string id)
        {
            ValidateSession();
            string result = "";
            try
            {
                var maxNumber = _orderLogic.GetList().Max(c => c.WayBillNumber);
                if (maxNumber != null)
                {
                    result = Convert.ToString(Convert.ToInt32(maxNumber) + 1);
                }
                else
                {
                    _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                    var configItem = _configurationLogic.GetSingleById(1).DeliveryWBNoStartFrom;
                    if (configItem != null)
                    {
                        result = configItem;
                    }

                }
            }
            catch (Exception ex)
            {
            }

            return Json(result);
        }

        public JsonResult GetTariffCostByParam(string jsonStringParam)
        {
            ValidateSession();

            decimal shipperCost = 0;
            decimal consigneeCost = 0;
            decimal returnedValue = 0;

            var jsonData = JObject.Parse(jsonStringParam);
            var shipperCityId = (int)jsonData.SelectToken("shipperCityId");
            var consigneeCityId = (int)jsonData.SelectToken("consigneeCityId");
            var deliveryOptionId = (int)jsonData.SelectToken("deliveryOptionId");
            var vehicleTypeId = (int)jsonData.SelectToken("vehicleTypeId");
            var unitTypeId = (int)jsonData.SelectToken("unitTypeId");
            var unitQuantity = (int)jsonData.SelectToken("unitQuantity");
            var weightScaleId = (int)jsonData.SelectToken("weightScaleId");
            var weightQuantity = (decimal)jsonData.SelectToken("weightQuantity");

            _tariffLogic = new Lms_TariffLogic(_cache, new EntityFrameworkGenericRepository<Lms_TariffPoco>(_dbContext));
            var tariffList = _tariffLogic.GetList();

            var shipperTariffInfo = tariffList.Where(c =>
                                                          c.CityId == shipperCityId && c.DeliveryOptionId == deliveryOptionId
                                                          && c.VehicleTypeId == vehicleTypeId && c.UnitTypeId == unitTypeId
                                                          //&& c.UptoWeight >= weightQuantity
                                                          ).ToList().FirstOrDefault();
            if (shipperTariffInfo != null)
            {
                if (unitQuantity > 0)
                {
                    shipperCost = (decimal)shipperTariffInfo.FirstUnitPrice;
                    if (unitQuantity > 1)
                    {
                        shipperCost = shipperCost + (unitQuantity - 1) * (decimal)shipperTariffInfo.PerUnitPrice;
                    }
                }
            }

            var consigneeTariffInfo = tariffList.Where(c =>
                                                          c.CityId == consigneeCityId && c.DeliveryOptionId == deliveryOptionId
                                                          && c.VehicleTypeId == vehicleTypeId && c.UnitTypeId == unitTypeId
                                                          //&& c.UptoWeight >= weightQuantity
                                                          ).ToList().FirstOrDefault();

            if (consigneeTariffInfo != null)
            {
                if (unitQuantity > 0)
                {
                    consigneeCost = (decimal)consigneeTariffInfo.FirstUnitPrice;
                    if (unitQuantity > 1)
                    {
                        consigneeCost = consigneeCost + (unitQuantity - 1) * (decimal)consigneeTariffInfo.PerUnitPrice;
                    }
                }
            }

            if (consigneeCost >= shipperCost)
            {
                returnedValue = consigneeCost;
            }
            else
            {
                returnedValue = shipperCost;
            }

            return Json(JsonConvert.SerializeObject(returnedValue));
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

        public JsonResult GetOrderShareByWayBillId(string id)
        {
            ValidateSession();

            try
            {
                var orderPocos = _orderLogic.GetList().Where(c => c.WayBillNumber == id).FirstOrDefault();

                return Json(JsonConvert.SerializeObject(orderPocos));
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

        public JsonResult GetReleaseStatusByOrderId(string id)
        {
            ValidateSession();
            string result = "";

            try
            {
                if (id != "")
                {
                    var orderId = _orderLogic.GetList().Where(c => c.WayBillNumber == id).FirstOrDefault().WayBillNumber;

                    if (!string.IsNullOrEmpty(orderId))
                    {
                        var _invoiceMappingLogic = new Lms_InvoiceWayBillMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_InvoiceWayBillMappingPoco>(_dbContext));
                        var waybillInfo = _invoiceMappingLogic.GetList().Where(c => c.WayBillNumber == orderId).FirstOrDefault();

                        if (waybillInfo != null)
                        {
                            if (waybillInfo.TotalWayBillAmount == 0)
                            {
                                result = "Released";
                            }
                            else
                            {
                                result = "";
                            }
                        }
                        else
                        {
                            result = "Released"; // given the option to modify not-invoiced order also.
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }

            return Json(result);
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

        public JsonResult GetOrderStatusByWaybillId(string id)
        {
            ValidateSession();

            try
            {
                var orderId = _orderLogic.GetList().Where(c => c.WayBillNumber == id).FirstOrDefault().Id;
                var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == Convert.ToInt32(orderId)).FirstOrDefault();

                return Json(JsonConvert.SerializeObject(orderStatus));
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }


        [HttpPost]
        public JsonResult GetCustomerReferenceNumberCount([FromBody]dynamic customerRefData)
        {
            ValidateSession();
            var result = 0;
            try
            {
                if (customerRefData != null)
                {
                    var custRefObject = (JObject)customerRefData;
                    var custRefNumber = custRefObject.SelectToken("custRef").ToString();
                    var orderId = custRefObject.SelectToken("orderId").ToString();

                    var orderList = _orderLogic.GetList().Where(c => !string.IsNullOrEmpty(c.ReferenceNumber)).ToList();
                    orderList = orderList.Where(c => (!string.IsNullOrEmpty(c.ReferenceNumber) ? c.ReferenceNumber.ToUpper() : "") == custRefNumber.ToUpper()).ToList();

                    if (Convert.ToInt32(orderId) > 0)
                    {
                        var existingOrder = _orderLogic.GetSingleById(Convert.ToInt32(orderId));
                        if (orderList.Count == 1)
                        {
                            if (existingOrder.Id != orderList.FirstOrDefault().Id)
                            {
                                result = orderList.Count;
                            }
                        }
                        if (orderList.Count > 1)
                        {
                            existingOrder.ReferenceNumber = !string.IsNullOrEmpty(existingOrder.ReferenceNumber) ? existingOrder.ReferenceNumber.ToUpper() : "";
                            if (existingOrder.ReferenceNumber != custRefNumber.ToUpper())
                            {
                                result = orderList.Count;
                            }
                        }
                    }
                    else
                    {
                        if (orderList.Count > 0)
                        {
                            result = orderList.Count;
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
        public JsonResult GetAwbCtnNumberCount([FromBody]dynamic awbCtnData)
        {
            ValidateSession();
            var result = 0;
            try
            {
                if (awbCtnData != null)
                {
                    var awbObject = (JObject)awbCtnData;
                    var awbCtnNumber = awbObject.SelectToken("awbCtn").ToString();
                    var orderId = awbObject.SelectToken("orderId").ToString();

                    var orderList = _orderLogic.GetList().Where(c => !string.IsNullOrEmpty(c.AwbCtnNumber)).ToList();
                    orderList = orderList.Where(c => (!string.IsNullOrEmpty(c.AwbCtnNumber) ? c.AwbCtnNumber.ToUpper() : "") == awbCtnNumber.ToUpper()).ToList();

                    if (Convert.ToInt32(orderId) > 0)
                    {
                        var existingOrder = _orderLogic.GetSingleById(Convert.ToInt32(orderId));
                        if (orderList.Count == 1)
                        {
                            if (existingOrder.Id != orderList.FirstOrDefault().Id)
                            {
                                result = orderList.Count;
                            }
                        }
                        if (orderList.Count > 1)
                        {
                            existingOrder.AwbCtnNumber = !string.IsNullOrEmpty(existingOrder.AwbCtnNumber) ? existingOrder.AwbCtnNumber.ToUpper() : "";
                            if (existingOrder.AwbCtnNumber != awbCtnNumber.ToUpper())
                            {
                                result = orderList.Count;
                            }
                        }
                    }
                    else
                    {
                        if (orderList.Count > 0)
                        {
                            result = orderList.Count;
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
        public JsonResult GetCargoCtlNumberCount([FromBody]dynamic cargoCtlData)
        {
            ValidateSession();
            var result = 0;
            try
            {
                if (cargoCtlData != null)
                {
                    var cargoObject = (JObject)cargoCtlData;
                    var cargoCtlNumber = cargoObject.SelectToken("cargoCtl").ToString();
                    var orderId = cargoObject.SelectToken("orderId").ToString();

                    var orderList = _orderLogic.GetList().Where(c => !string.IsNullOrEmpty(c.CargoCtlNumber)).ToList();
                    orderList = orderList.Where(c => (!string.IsNullOrEmpty(c.CargoCtlNumber) ? c.CargoCtlNumber.ToUpper() : "") == cargoCtlNumber.ToUpper()).ToList();

                    if (Convert.ToInt32(orderId) > 0)
                    {
                        var existingOrder = _orderLogic.GetSingleById(Convert.ToInt32(orderId));
                        if (orderList.Count == 1)
                        {
                            if (existingOrder.Id != orderList.FirstOrDefault().Id)
                            {
                                result = orderList.Count;
                            }
                        }
                        if (orderList.Count > 1)
                        {
                            existingOrder.CargoCtlNumber = !string.IsNullOrEmpty(existingOrder.CargoCtlNumber) ? existingOrder.CargoCtlNumber.ToUpper() : "";
                            if (existingOrder.CargoCtlNumber != cargoCtlNumber.ToUpper())
                            {
                                result = orderList.Count;
                            }
                        }
                    }
                    else
                    {
                        if (orderList.Count > 0)
                        {
                            result = orderList.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(result);
        }

        public JsonResult PrintWaybillAsPdf([FromBody]dynamic orderData)
        {
            try
            {
                List<ViewModel_PrintWaybill> waybillPrintViewModels = new List<ViewModel_PrintWaybill>();
                JArray wayBillNumberList = null;

                var orderList = _orderLogic.GetList();
                var orderStatusList = _orderStatusLogic.GetList();

                _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
                var orderAdditionalServices = _orderAdditionalServiceLogic.GetList();

                _additionalServiceLogic = new Lms_AdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>(_dbContext));
                var additionalServices = _additionalServiceLogic.GetList();

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

                _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
                var employeeList = _employeeLogic.GetList();

                if (orderData != null)
                {
                    wayBillNumberList = JArray.Parse(JsonConvert.SerializeObject(orderData[0]));
                }

                var printOption = (JObject)orderData[1];
                var numberOfCopyOnPage = printOption.SelectToken("numberOfcopyOnEachPage").ToString();
                var numberOfCopyPerItem = printOption.SelectToken("numberOfcopyPerItem").ToString();
                var ignorePrice = Convert.ToBoolean(printOption.SelectToken("ignorePrice"));
                var isMiscellaneous = Convert.ToBoolean(printOption.SelectToken("isMiscellaneous"));
                var orderTypeId = Convert.ToBoolean(printOption.SelectToken("orderTypeId"));
                var viewName = printOption.SelectToken("viewName").ToString();

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

                foreach (var item in wayBillNumberList)
                {
                    var wbNumber = item.ToString().Trim();
                    var waybillOrderList = orderList.Where(c => c.WayBillNumber == wbNumber).ToList(); //loads all order
                    foreach (var orderInfo in waybillOrderList)
                    {
                        if (orderInfo != null)
                        {
                            ViewModel_PrintWaybill waybillPrintViewModel = new ViewModel_PrintWaybill();

                            if (orderInfo.OrderTypeId == (int)OrderType.MiscellaneousOrder)
                            {
                                isMiscellaneous = true;
                            }
                            else
                            {
                                isMiscellaneous = false;
                            }

                            waybillPrintViewModel.WaybillNumber = orderInfo.WayBillNumber;
                            if (orderInfo.ScheduledPickupDate != null)
                            {
                                waybillPrintViewModel.WayBillDate = ((DateTime)orderInfo.ScheduledPickupDate).ToString("dd-MMM-yy").ToUpper();
                            }
                            else
                            {
                                //waybillPrintViewModel.WayBillDate = orderInfo.CreateDate.ToString("dd-MMM-yy");
                            }

                            waybillPrintViewModel.BillerCustomerId = orderInfo.BillToCustomerId;
                            waybillPrintViewModel.CustomerRefNo = !string.IsNullOrEmpty(orderInfo.ReferenceNumber) ? orderInfo.ReferenceNumber.ToUpper() : orderInfo.ReferenceNumber;
                            waybillPrintViewModel.CargoCtlNo = !string.IsNullOrEmpty(orderInfo.CargoCtlNumber) ? orderInfo.CargoCtlNumber.ToUpper() : orderInfo.CargoCtlNumber; 
                            waybillPrintViewModel.AwbContainerNo = !string.IsNullOrEmpty(orderInfo.AwbCtnNumber) ? orderInfo.AwbCtnNumber.ToUpper() : orderInfo.AwbCtnNumber; 
                            waybillPrintViewModel.PickupRefNo = !string.IsNullOrEmpty(orderInfo.PickupReferenceNumber) ? orderInfo.PickupReferenceNumber.ToUpper() : orderInfo.PickupReferenceNumber; 
                            waybillPrintViewModel.DeliveryRefNo = !string.IsNullOrEmpty(orderInfo.DeliveryReferenceNumber) ? orderInfo.DeliveryReferenceNumber.ToUpper() : orderInfo.DeliveryReferenceNumber;
                            waybillPrintViewModel.BillerCustomerName = customers.Where(c => c.Id == orderInfo.BillToCustomerId).FirstOrDefault().CustomerName.ToUpper();
                            waybillPrintViewModel.OrderedByName = !string.IsNullOrEmpty(orderInfo.OrderedBy) ? orderInfo.OrderedBy.ToUpper() : orderInfo.OrderedBy; 
                            if (orderInfo.DeliveryOptionId != null && orderInfo.DeliveryOptionId > 0)
                            {
                                waybillPrintViewModel.DeliveryOptionShortCode = deliveryOptions.Where(c => c.Id == orderInfo.DeliveryOptionId).FirstOrDefault().ShortCode.ToUpper();
                            }
                            waybillPrintViewModel.NumberOfCopyOnEachPage = numberOfCopyOnPage == "" ? 0 : Convert.ToInt32(numberOfCopyOnPage);
                            waybillPrintViewModel.NumberOfCopyPerItem = numberOfCopyPerItem == "" ? 0 : Convert.ToInt32(numberOfCopyPerItem);

                            waybillPrintViewModel.OrderBasePrice = orderInfo.OrderBasicCost.ToString();
                            if (orderInfo.BasicCostOverriden != null && orderInfo.BasicCostOverriden > 0)
                            {
                                waybillPrintViewModel.OrderBasePrice = orderInfo.BasicCostOverriden.ToString();
                            }

                            waybillPrintViewModel.FuelSurcharge = "0.00";
                            if (orderInfo.FuelSurchargePercentage != null && orderInfo.FuelSurchargePercentage > 0)
                            {
                                var fuelAmnt = Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) * orderInfo.FuelSurchargePercentage / 100;
                                if (fuelAmnt > 0)
                                {
                                    waybillPrintViewModel.FuelSurcharge = ((decimal)fuelAmnt).ToString("0.00");
                                }
                            }

                            waybillPrintViewModel.OrderDiscountAmount = "0.00";
                            if (orderInfo.DiscountPercentOnOrderCost != null && orderInfo.DiscountPercentOnOrderCost > 0)
                            {
                                var orderDiscount = ((Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) + Convert.ToDecimal(waybillPrintViewModel.FuelSurcharge)) * orderInfo.DiscountPercentOnOrderCost / 100);
                                if (orderDiscount > 0)
                                {
                                    waybillPrintViewModel.OrderDiscountAmount = ((decimal)orderDiscount).ToString("0.00");
                                }
                            }

                            decimal orderTotalTax = 0; // for misc. orders
                            waybillPrintViewModel.AdditionalServiceCost = "0.00";
                            waybillPrintViewModel.AdditionalServices = additionalServices;
                            waybillPrintViewModel.OrderAdditionalServices = null;
                            if (orderInfo.TotalAdditionalServiceCost > 0)
                            {
                                waybillPrintViewModel.AdditionalServiceCost = Convert.ToDecimal(orderInfo.TotalAdditionalServiceCost).ToString("0.00");
                                var addServices = orderAdditionalServices.Where(c => c.OrderId == orderInfo.Id).ToList();
                                waybillPrintViewModel.OrderAdditionalServices = addServices;

                                if (isMiscellaneous == true)
                                {
                                    foreach (var addservice in addServices)
                                    {
                                        var serviceCost = addservice.AdditionalServiceFee;
                                        if (orderInfo.DiscountPercentOnOrderCost != null && orderInfo.DiscountPercentOnOrderCost > 0)
                                        {
                                            serviceCost = serviceCost - (serviceCost * (decimal)orderInfo.DiscountPercentOnOrderCost / 100);
                                        }
                                        if (addservice.IsTaxAppliedOnAddionalService == true && addservice.TaxAmountOnAdditionalService != null && addservice.TaxAmountOnAdditionalService > 0)
                                        {
                                            orderTotalTax += serviceCost * (decimal)addservice.TaxAmountOnAdditionalService / 100;
                                        }
                                    }
                                }
                            }

                            waybillPrintViewModel.OrderTaxAmountOnBasePrice = "0.00";

                            if (isMiscellaneous == true)
                            {
                                waybillPrintViewModel.OrderTaxAmountOnBasePrice = Convert.ToDecimal(orderTotalTax).ToString("0.00");
                                waybillPrintViewModel.NetTotalOrderCost = Convert.ToDecimal(waybillPrintViewModel.AdditionalServiceCost).ToString("0.00");
                                waybillPrintViewModel.AdditionalServiceCost = "0.00"; //For misc. waybill print 
                            }
                            else
                            {
                                decimal taxAmnt = 0;
                                if (orderInfo.ApplicableGstPercent != null && orderInfo.ApplicableGstPercent > 0)
                                {
                                    taxAmnt = (Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) + Convert.ToDecimal(waybillPrintViewModel.FuelSurcharge) - Convert.ToDecimal(waybillPrintViewModel.OrderDiscountAmount)) * (decimal)orderInfo.ApplicableGstPercent / 100;
                                    if (taxAmnt > 0)
                                    {
                                        waybillPrintViewModel.OrderTaxAmountOnBasePrice = Convert.ToDecimal(taxAmnt).ToString("0.00");
                                    }
                                }
                                waybillPrintViewModel.NetTotalOrderCost = (taxAmnt + Convert.ToDecimal(waybillPrintViewModel.OrderBasePrice) + Convert.ToDecimal(waybillPrintViewModel.FuelSurcharge) - Convert.ToDecimal(waybillPrintViewModel.OrderDiscountAmount) + Convert.ToDecimal(waybillPrintViewModel.AdditionalServiceCost)).ToString("0.00");
                            }

                            waybillPrintViewModel.ShipperCustomerName = customers.Where(c => c.Id == orderInfo.ShipperCustomerId).FirstOrDefault().CustomerName.ToUpper();
                            var shippperAddress = addresses.Where(c => c.Id == orderInfo.ShipperAddressId).FirstOrDefault();
                            waybillPrintViewModel.ShipperCustomerAddressLine1 = !string.IsNullOrEmpty(shippperAddress.UnitNumber) ? shippperAddress.UnitNumber.ToUpper() + ", " + shippperAddress.AddressLine.ToUpper() : shippperAddress.AddressLine.ToUpper();
                            waybillPrintViewModel.ShipperCustomerAddressLine2 = cities.Where(c => c.Id == shippperAddress.CityId).FirstOrDefault().CityName.ToUpper() + ", " + provinces.Where(c => c.Id == shippperAddress.ProvinceId).FirstOrDefault().ShortCode.ToUpper() + "  " + (!string.IsNullOrEmpty(shippperAddress.PostCode) ? shippperAddress.PostCode.ToUpper() : shippperAddress.PostCode); 

                            if (isMiscellaneous == false)
                            {
                                waybillPrintViewModel.ConsigneeCustomerName = customers.Where(c => c.Id == orderInfo.ConsigneeCustomerId).FirstOrDefault().CustomerName.ToUpper();
                                var consigneeAddress = addresses.Where(c => c.Id == orderInfo.ConsigneeAddressId).FirstOrDefault();
                                waybillPrintViewModel.ConsigneeCustomerAddressLine1 = !string.IsNullOrEmpty(consigneeAddress.UnitNumber) ? consigneeAddress.UnitNumber.ToUpper() + ", " + consigneeAddress.AddressLine.ToUpper() : consigneeAddress.AddressLine.ToUpper();
                                waybillPrintViewModel.ConsigneeCustomerAddressLine2 = cities.Where(c => c.Id == consigneeAddress.CityId).FirstOrDefault().CityName.ToUpper() + ", " + provinces.Where(c => c.Id == consigneeAddress.ProvinceId).FirstOrDefault().ShortCode.ToUpper() + "  " + (!string.IsNullOrEmpty(consigneeAddress.PostCode) ? consigneeAddress.PostCode.ToUpper() : consigneeAddress.PostCode);
                            }

                            waybillPrintViewModel.SkidQuantity = orderInfo.SkidQuantity;
                            waybillPrintViewModel.TotalSkidPieces = orderInfo.TotalPiece;

                            if (orderInfo.UnitQuantity > 0)
                            {
                                waybillPrintViewModel.UnitQuantity = orderInfo.UnitQuantity;
                                if (orderInfo.UnitTypeId > 0)
                                {
                                    waybillPrintViewModel.UnitTypeName = unitTypes.Where(c => c.Id == orderInfo.UnitTypeId).FirstOrDefault().TypeName.ToUpper();
                                    waybillPrintViewModel.UnitTypeShortCode = unitTypes.Where(c => c.Id == orderInfo.UnitTypeId).FirstOrDefault().ShortCode.ToUpper();
                                }
                            }

                            if (orderInfo.WeightTotal > 0)
                            {
                                waybillPrintViewModel.WeightTotal = orderInfo.WeightTotal.ToString();
                                if (orderInfo.WeightScaleId > 0)
                                {
                                    waybillPrintViewModel.WeightScaleShortCode = weightScales.Where(c => c.Id == orderInfo.WeightScaleId).FirstOrDefault().ShortCode;
                                }
                            }

                            waybillPrintViewModel.DeliveryDate = null;
                            waybillPrintViewModel.DeliveryTime = null;
                            waybillPrintViewModel.PUDriverName = "";

                            var orderStatus = orderStatusList.Where(c => c.OrderId == orderInfo.Id).FirstOrDefault();
                            if (orderStatus != null)
                            {
                                waybillPrintViewModel.PUDriverNum = orderStatus.DispatchedToEmployeeId.ToString();
                                if (!string.IsNullOrEmpty(waybillPrintViewModel.PUDriverNum))
                                {
                                    waybillPrintViewModel.PUDriverName = employeeList.Where(c => c.Id == Convert.ToInt32(waybillPrintViewModel.PUDriverNum)).FirstOrDefault().FirstName.ToUpper();
                                }

                                if (orderStatus.PassedOffToEmployeeId != null && orderStatus.PassedOffToEmployeeId > 0)
                                {
                                    waybillPrintViewModel.DeliveryDriverName = employeeList.Where(c => c.Id == orderStatus.PassedOffToEmployeeId).FirstOrDefault().FirstName.ToUpper();
                                    waybillPrintViewModel.DeliveryDriverNum = orderStatus.PassedOffToEmployeeId.ToString();
                                }
                                else if (orderStatus.DispatchedToEmployeeId != null && orderStatus.DispatchedToEmployeeId > 0)
                                {
                                    waybillPrintViewModel.DeliveryDriverName = employeeList.Where(c => c.Id == orderStatus.DispatchedToEmployeeId).FirstOrDefault().FirstName.ToUpper();
                                    waybillPrintViewModel.DeliveryDriverNum = orderStatus.DispatchedToEmployeeId.ToString();
                                }
                                waybillPrintViewModel.ReceivedBy = !string.IsNullOrEmpty(orderStatus.ReceivedByName) ? orderStatus.ReceivedByName.ToUpper() : orderStatus.ReceivedByName;

                                if (!string.IsNullOrEmpty(waybillPrintViewModel.ReceivedBy))
                                {
                                    if (waybillPrintViewModel.ReceivedBy.Length > 15)
                                    {
                                        waybillPrintViewModel.ReceivedBy = waybillPrintViewModel.ReceivedBy.Substring(0, 12) + "...";
                                    }
                                }

                                if (orderStatus.DeliveredDatetime != null)
                                {
                                    waybillPrintViewModel.DeliveryDate = ((DateTime)orderStatus.DeliveredDatetime).ToString("dd-MMM-yyyy").ToUpper();
                                    waybillPrintViewModel.DeliveryTime = ((DateTime)orderStatus.DeliveredDatetime).ToString("hh:mm tt").ToUpper();
                                }

                            }
                            if (orderInfo.IsPrintedOnWayBill != null && orderInfo.IsPrintedOnWayBill == true)
                            {
                                waybillPrintViewModel.WaybillComments = !string.IsNullOrEmpty(orderInfo.CommentsForWayBill) ? orderInfo.CommentsForWayBill.ToUpper() : orderInfo.CommentsForWayBill;
                            }

                            if (isMiscellaneous == true)
                            {
                                if (orderInfo.ServiceProviderEmployeeId != null && orderInfo.ServiceProviderEmployeeId > 0)
                                {
                                    waybillPrintViewModel.DeliveryDriverName = employeeList.Where(c => c.Id == orderInfo.ServiceProviderEmployeeId).FirstOrDefault().FirstName.ToUpper();
                                }
                            }

                            if (ignorePrice == true)
                            {
                                waybillPrintViewModel.OrderBasePrice = "";
                                waybillPrintViewModel.FuelSurcharge = "";
                                waybillPrintViewModel.OrderDiscountAmount = "";
                                waybillPrintViewModel.AdditionalServiceCost = "";
                                waybillPrintViewModel.OrderTaxAmountOnBasePrice = "";
                                waybillPrintViewModel.NetTotalOrderCost = "";
                                waybillPrintViewModel.OrderAdditionalServices = null;
                            }
                            else
                            {
                                if (Convert.ToDouble(waybillPrintViewModel.OrderDiscountAmount) <= 0)
                                {
                                    waybillPrintViewModel.OrderDiscountAmount = "";
                                }

                                if (Convert.ToDouble(waybillPrintViewModel.AdditionalServiceCost) <= 0)
                                {
                                    waybillPrintViewModel.AdditionalServiceCost = "";
                                }
                            }

                            waybillPrintViewModels.Add(waybillPrintViewModel);
                        }
                    }
                }

                var webrootPath = _hostingEnvironment.WebRootPath;
                var uniqueId = DateTime.Now.ToFileTime();
                var fileName = "waybill_" + uniqueId + ".pdf";
                var directoryPath = webrootPath + "/contents/waybills/";
                var filePath = directoryPath + fileName;

                if (!System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                }

                var pdfReport = new ViewAsPdf(viewName, waybillPrintViewModels)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.Letter
                };
                var file = pdfReport.BuildFile(ControllerContext).Result;

                System.IO.File.WriteAllBytes(filePath, file);

                string returnPath = "/contents/waybills/" + fileName;

                //_emailService.SendEmail("zizaheer@yahoo.com", "test subject", "test body content", returnPath);

                return Json(returnPath);
            }
            catch (Exception ex)
            {
                return null;
            }
            //return View();
        }

        public void SendDispatchEmail(string firstName, string emailAddress, string waybillNumbers)
        {
            try
            {
                string[] waybills = waybillNumbers.Split(",");
                StringBuilder emailBody = new StringBuilder();
                emailBody.Append("<br />" + "Please find order dispatch information below: <br />" + Environment.NewLine + Environment.NewLine);
                _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
                _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
                _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
                _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));

                foreach (var waybill in waybills)
                {
                    var orderList = _orderLogic.GetList().Where(c => c.WayBillNumber == waybill.Trim()).ToList();
                    foreach (var order in orderList)
                    {
                        var sipperAddress = _addressLogic.GetSingleById((int)order.ShipperAddressId);
                        var consigneeAddress = _addressLogic.GetSingleById((int)order.ConsigneeAddressId);

                        if (!string.IsNullOrEmpty(sipperAddress.UnitNumber))
                        {
                            sipperAddress.UnitNumber = sipperAddress.UnitNumber + ", ";
                        }
                        if (!string.IsNullOrEmpty(consigneeAddress.UnitNumber))
                        {
                            consigneeAddress.UnitNumber = consigneeAddress.UnitNumber + ", ";
                        }

                        emailBody.Append("ORDER # <b>" + order.WayBillNumber + "</b>" + Environment.NewLine);
                        //emailBody.Append("==============================================" + Environment.NewLine);
                        emailBody.Append("<table border=2><tr><td>  </td></tr></table>");
                        emailBody.Append("<table border=2><tr><td>  </td></tr></table>");



                        emailBody.Append("PICKUP FROM:  ");
                        emailBody.Append(_customerLogic.GetSingleById((int)order.ShipperCustomerId).CustomerName + Environment.NewLine);
                        emailBody.Append(sipperAddress.UnitNumber + sipperAddress.AddressLine + Environment.NewLine);
                        emailBody.Append(_cityLogic.GetSingleById(sipperAddress.CityId).CityName + ", " + _provinceLogic.GetSingleById(sipperAddress.ProvinceId).ShortCode + "  " + sipperAddress.PostCode + Environment.NewLine);
                        emailBody.Append("* * * * * * * * * *" + Environment.NewLine);
                        emailBody.Append("DELIVER TO:  ");
                        emailBody.Append(_customerLogic.GetSingleById((int)order.ConsigneeCustomerId).CustomerName + Environment.NewLine);
                        emailBody.Append(consigneeAddress.UnitNumber + consigneeAddress.AddressLine + Environment.NewLine);
                        emailBody.Append(_cityLogic.GetSingleById(consigneeAddress.CityId).CityName + ", " + _provinceLogic.GetSingleById(consigneeAddress.ProvinceId).ShortCode + "  " + consigneeAddress.PostCode + Environment.NewLine);
                        emailBody.Append("* * * * * * * * * *" + Environment.NewLine);
                        var statusUpdateLink = "";
                        if (HttpContext.Request.IsHttps == true)
                        {
                            statusUpdateLink = "https://";
                        }
                        else
                        {
                            statusUpdateLink = "http://";
                        }
                        statusUpdateLink = statusUpdateLink + HttpContext.Request.Host + "/UpdateDelivery/OrderId=" + order.Id.ToString();
                        emailBody.Append("Update delivery status link: " + "<a href='" + statusUpdateLink + "'> Click Here </a>" + Environment.NewLine);
                    }

                    emailBody.Append(Environment.NewLine + Environment.NewLine);
                }

                _emailService.SendEmail(firstName, emailAddress, 1, "", "", emailBody.ToString());
            }
            catch (Exception ex)
            {

            }

        }

        private ViewModel_DeliveryOrder GetAllRequiredDataForDispatchBoard()
        {
            var deliveryOrderViewModel = GetDeliveryOrderRelatedAdditionalData();
            deliveryOrderViewModel.DispatchedOrders = GetDispatchedOrders(deliveryOrderViewModel);

            return deliveryOrderViewModel;
        }

        private ViewModel_DeliveryOrder GetDeliveryOrderRelatedAdditionalData()
        {
            #region Get relevant data for a new order

            ViewModel_DeliveryOrder deliveryOrderViewModel = new ViewModel_DeliveryOrder();

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            deliveryOrderViewModel.Cities = _cityLogic.GetList();

            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            deliveryOrderViewModel.Provinces = _provinceLogic.GetList();

            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
            deliveryOrderViewModel.Configuration = _configurationLogic.GetList().FirstOrDefault();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            deliveryOrderViewModel.Customers = _customerLogic.GetList();

            _customerAddressLogic = new Lms_CustomerAddressMappingLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerAddressMappingPoco>(_dbContext));
            var custWithBillingAddress = _customerAddressLogic.GetList().Where(c => c.AddressTypeId == (int)Enum_AddressType.Billing).ToList();

            deliveryOrderViewModel.BillingCustomers = (from cust in deliveryOrderViewModel.Customers
                                                       join add in custWithBillingAddress on cust.Id equals add.CustomerId
                                                       select cust).ToList();

            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(_cache, new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            deliveryOrderViewModel.DeliveryOptions = _deliveryOptionLogic.GetList();

            _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
            deliveryOrderViewModel.UnitTypes = _unitTypeLogic.GetList();

            _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));
            deliveryOrderViewModel.WeightScales = _weightScaleLogic.GetList();

            _additionalServiceLogic = new Lms_AdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>(_dbContext));
            deliveryOrderViewModel.AdditionalServices = _additionalServiceLogic.GetList();

            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            deliveryOrderViewModel.Employees = _employeeLogic.GetList();

            if (deliveryOrderViewModel.Configuration.IsSignInRequiredForDispatch != null)
            {
                if ((bool)deliveryOrderViewModel.Configuration.IsSignInRequiredForDispatch)
                {

                    var _timesheetLogic = new Lms_EmployeeTimesheetLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeTimesheetPoco>(_dbContext));
                    var signedInEmployees = _timesheetLogic.GetList().Where(c => c.SignInDatetime != null && c.SignOutDatetime == null).ToList();

                    deliveryOrderViewModel.Employees = (from employees in deliveryOrderViewModel.Employees
                                                        join signedIn in signedInEmployees on employees.Id equals signedIn.EmployeeId
                                                        select employees).ToList();

                }
            }


            return deliveryOrderViewModel;

            #endregion

        }

        private List<ViewModel_OrderDispatched> GetDispatchedOrders(ViewModel_DeliveryOrder deliveryOrderViewModel)
        {
            #region get datatable for dispatch board

            List<ViewModel_OrderDispatched> dispatchedOrders = new List<ViewModel_OrderDispatched>();
            var orders = _orderLogic.GetList().Where(c => c.IsInvoiced == false && (c.TotalOrderCost + c.TotalAdditionalServiceCost) > 0).ToList(); //Load all orders 
            var ordersStatus = _orderStatusLogic.GetList();

            var filteredOrdersForDispatchBoard = (from order in orders
                                                  join status in ordersStatus on order.Id equals status.OrderId
                                                  where status.IsDelivered != true
                                                  select new { order, status }).ToList();

            foreach (var item in filteredOrdersForDispatchBoard)
            {
                ViewModel_OrderDispatched data = new ViewModel_OrderDispatched();
                data.OrderId = item.order.Id;
                data.OrderTypeId = item.order.OrderTypeId;
                data.OrderTypeFlag = data.OrderTypeId == 1 ? "S" : data.OrderTypeId == 2 ? "R" : "";
                data.WayBillNumber = item.order.WayBillNumber;
                if (item.order.ScheduledPickupDate != null)
                {
                    data.OrderDateString = ((DateTime)item.order.ScheduledPickupDate).ToString("dd-MMM-yy");
                }
                else
                {
                    data.OrderDateString = item.order.CreateDate.ToString("dd-MMM-yy");
                }

                data.DeliveryOptionId = (int)item.order.DeliveryOptionId;
                data.DeliveryOptionName = deliveryOrderViewModel.DeliveryOptions.Where(c => c.Id == data.DeliveryOptionId).FirstOrDefault().OptionName;
                data.DeliveryOptionCode = deliveryOrderViewModel.DeliveryOptions.Where(c => c.Id == data.DeliveryOptionId).FirstOrDefault().ShortCode;

                data.CustomerRefNumber = item.order.ReferenceNumber;
                data.UnitTypeId = item.order.UnitTypeId;
                if (data.UnitTypeId > 0)
                {
                    data.UnitTypeName = deliveryOrderViewModel.UnitTypes.Where(c => c.Id == data.UnitTypeId).FirstOrDefault().ShortCode;
                }

                data.UnitQuantity = item.order.UnitQuantity;
                data.SkidQuantity = item.order.SkidQuantity;
                data.TotalPiece = item.order.TotalPiece;
                data.SpcIns = "";
                data.ShipperCustomerId = (int)item.order.ShipperCustomerId;
                data.ShipperCustomerName = deliveryOrderViewModel.Customers.Where(c => c.Id == data.ShipperCustomerId).FirstOrDefault().CustomerName;
                data.ConsigneeCustomerId = (int)item.order.ConsigneeCustomerId;
                data.ConsigneeCustomerName = deliveryOrderViewModel.Customers.Where(c => c.Id == data.ConsigneeCustomerId).FirstOrDefault().CustomerName;
                data.BillerCustomerId = item.order.BillToCustomerId;
                data.BillerCustomerName = deliveryOrderViewModel.Customers.Where(c => c.Id == data.BillerCustomerId).FirstOrDefault().CustomerName;

                if (item.status.IsDispatched == null || item.status.IsDispatched == false)
                {
                    data.OrderStatus = "0"; // 0 represents not yet dispatched; just the order is created 

                }
                else if (item.status.IsDispatched == true && (item.status.IsPickedup == null || item.status.IsPickedup == false))
                {
                    data.OrderStatus = "WFP"; // WFP - Waiting for pickup
                    data.RowColorCode = "#f9e6e0";
                }
                else if (item.status.IsPickedup == true && (item.status.IsDelivered == null || item.status.IsDelivered == false))
                {
                    data.OrderStatus = "WFD"; // WFD - Waiting for delivery
                    data.RowColorCode = "#fbffbd";
                }
                else if (item.status.IsDelivered == true)
                {
                    data.OrderStatus = "WFB"; // WFB - Waiting for bill
                    data.RowColorCode = "#ccffc6";
                }

                data.DispatchedToEmployeeId = item.status.DispatchedToEmployeeId;
                if (data.DispatchedToEmployeeId != null)
                {
                    if (item.status.PassedOffToEmployeeId != null)
                    {
                        data.DispatchedToEmployeeId = item.status.PassedOffToEmployeeId;
                    }

                    var empInfo = deliveryOrderViewModel.Employees.Where(c => c.Id == data.DispatchedToEmployeeId).FirstOrDefault();
                    data.DispatchedToEmployeeName = empInfo.FirstName + " " + empInfo.LastName;

                    if (!string.IsNullOrEmpty(empInfo.MobileNumber))
                    {
                        data.DispatchedToEmployeePhone = empInfo.MobileNumber;
                    }
                    else if (!string.IsNullOrEmpty(empInfo.PhoneNumber))
                    {
                        data.DispatchedToEmployeePhone = empInfo.PhoneNumber;
                    }

                    data.DispatchedToEmployeeEmail = empInfo.EmailAddress;
                }

                dispatchedOrders.Add(data);
            }

            return dispatchedOrders;

            #endregion
        }

        private string CreateDeliveryOrder(Lms_OrderPoco orderPoco, List<Lms_OrderAdditionalServicePoco> orderAdditionalServices)
        {
            string result = "";
            string trackingNumber = "";
            int orderId = 0;
            string newWaybillNumber = "";
            try
            {
                var waybillPrefix = "";

                _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
                var configInfo = _configurationLogic.GetSingleById(1);

                _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));

                using (var scope = new TransactionScope())
                {
                    newWaybillNumber = orderPoco.WayBillNumber;

                    if (string.IsNullOrEmpty(newWaybillNumber) || Convert.ToInt32(newWaybillNumber) < 1)
                    {
                        var maxWaybillNumber = _orderLogic.GetList().Select(c => c.WayBillNumber).OrderByDescending(c => c).FirstOrDefault();
                        if (maxWaybillNumber != null)
                        {
                            newWaybillNumber = Convert.ToString(Convert.ToInt32(maxWaybillNumber) + 1);
                        }
                        else
                        {
                            newWaybillNumber = configInfo.DeliveryWBNoStartFrom;
                            if (string.IsNullOrEmpty(newWaybillNumber))
                            {
                                newWaybillNumber = "1";
                            }
                        }

                        waybillPrefix = configInfo.WayBillPrefix;
                        newWaybillNumber = waybillPrefix + newWaybillNumber;

                        if (orderPoco.OrderTypeId == 1)
                        {
                            orderPoco.WayBillNumber = newWaybillNumber;
                        }
                    }

                    orderPoco.IsInvoiced = false;
                    orderPoco.IsPrePrinted = false;
                    orderPoco.CreateDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
                    orderId = _orderLogic.Add(orderPoco).Id;

                    foreach (var addService in orderAdditionalServices)
                    {
                        addService.OrderId = orderId;
                        _orderAdditionalServiceLogic.Add(addService);
                    }

                    string sequecNo = DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString() + orderId.ToString();
                    if (orderPoco.OrderTypeId == 1)
                    {
                        trackingNumber = "TRK01-" + sequecNo;
                    }
                    else if (orderPoco.OrderTypeId == 2)
                    {
                        trackingNumber = "TRK02-" + sequecNo;
                    }

                    Lms_OrderStatusPoco orderStatusPoco = new Lms_OrderStatusPoco();
                    orderStatusPoco.OrderId = orderId;
                    orderStatusPoco.TrackingNumber = trackingNumber;
                    orderStatusPoco.StatusLastUpdatedOn = DateTime.Now;
                    orderStatusPoco.CreateDate = orderPoco.CreateDate;

                    _orderStatusLogic.Add(orderStatusPoco);

                    if (orderId > 0)
                    {
                        scope.Complete();
                    }

                    var orderInfo = new
                    {
                        orderId = orderId.ToString(),
                        waybillNumber = newWaybillNumber
                    };

                    result = JsonConvert.SerializeObject(orderInfo);
                }

            }
            catch (Exception ex)
            {

            }

            return result;

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