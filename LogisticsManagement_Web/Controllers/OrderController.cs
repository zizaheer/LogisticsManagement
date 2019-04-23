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
    public class OrderController : Controller
    {

        //private IMemoryCache _cache;  // To do later 

        private Lms_OrderLogic _orderLogic;
        private Lms_OrderStatusLogic _orderStatusLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private App_CityLogic _cityLogic;
        private App_ProvinceLogic _provinceLogic;
        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private Lms_UnitTypeLogic _unitTypeLogic;
        private Lms_WeightScaleLogic _weightScaleLogic;
        private Lms_OrderAdditionalServiceLogic _orderAdditionalServiceLogic;
        private Lms_AdditionalServiceLogic _additionalServiceLogic;
        private Lms_ConfigurationLogic _configurationLogic;
        private Lms_TariffLogic _tariffLogic;
        private Lms_EmployeeLogic _employeeLogic;
        private Lms_EmployeeTimesheetLogic _employeeTimesheetLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public OrderController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();
            return View(GetOrderData());
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewOrderData", GetOrderData());
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
                    List<Lms_OrderAdditionalServicePoco> orderAdditionalServices = JsonConvert.DeserializeObject<List<Lms_OrderAdditionalServicePoco>>(JsonConvert.SerializeObject(orderData[1]));

                    if (orderPoco.Id < 1 && orderPoco.BillToCustomerId > 0 && orderPoco.ShipperCustomerId > 0 && orderPoco.ConsigneeCustomerId > 0)
                    {
                        orderPoco.CreatedBy = sessionData.UserId;
                        var orderInfo = _orderLogic.CreateNewOrder(orderPoco, orderAdditionalServices);
                        if (!string.IsNullOrEmpty(orderInfo))
                        {
                            var jObject = JObject.Parse(orderInfo);
                            var returnedObject = (string)jObject.SelectToken("ReturnedValue");

                            if (returnedObject.Length > 0)
                            {
                                result = returnedObject;
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

                    var existingOrder = new Lms_OrderPoco();

                    if (orderPoco != null && orderPoco.OrderTypeId > 0)
                    {
                        existingOrder = _orderLogic.GetList().Where(c => c.OrderTypeId == orderPoco.OrderTypeId && c.WayBillNumber == orderPoco.WayBillNumber).FirstOrDefault();
                    }

                    if (existingOrder != null && existingOrder.Id > 0)
                    {
                        existingOrder.ReferenceNumber = orderPoco.ReferenceNumber;
                        existingOrder.CargoCtlNumber = orderPoco.CargoCtlNumber;
                        existingOrder.AwbCtnNumber = orderPoco.CargoCtlNumber;
                        existingOrder.ShipperCustomerId = orderPoco.ShipperCustomerId;
                        existingOrder.ConsigneeCustomerId = orderPoco.ConsigneeCustomerId;
                        existingOrder.BillToCustomerId = orderPoco.BillToCustomerId;
                        existingOrder.ScheduledPickupDate = orderPoco.ScheduledPickupDate;

                        existingOrder.CityId = orderPoco.CityId;
                        existingOrder.DeliveryOptionId = orderPoco.DeliveryOptionId;
                        existingOrder.VehicleTypeId = orderPoco.VehicleTypeId;
                        existingOrder.WeightScaleId = orderPoco.WeightScaleId;
                        existingOrder.WeightTotal = orderPoco.WeightTotal;
                        existingOrder.UnitQuantity = orderPoco.UnitQuantity;
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
                        var orderInfo = _orderLogic.CreateNewOrder(orderPoco, orderAdditionalServices);
                        if (!string.IsNullOrEmpty(orderInfo))
                        {
                            var jObject = JObject.Parse(orderInfo);
                            var returnedObject = (string)jObject.SelectToken("ReturnedValue");

                            if (returnedObject.Length > 0)
                            {
                                result = returnedObject;
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
        public IActionResult Remove(string id)
        {
            bool result = false;
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

                    result = true;
                }

            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }

        public JsonResult GetTariffCostByParam(string jsonStringParam)
        {
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
                                                          && c.WeightScaleId == weightScaleId && c.UptoWeight >= weightQuantity
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
                                                          && c.WeightScaleId == weightScaleId && c.UptoWeight >= weightQuantity
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

        public JsonResult GetOrderByWayBillId(string id)
        {
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

        private DeliveryOrderViewModel GetOrderData()
        {
            DeliveryOrderViewModel deliveryOrderViewModel = new DeliveryOrderViewModel();
            deliveryOrderViewModel.Orders = _orderLogic.GetList().Where(c => c.OrderTypeId == 1).ToList();

            _cityLogic = new App_CityLogic(_cache, new EntityFrameworkGenericRepository<App_CityPoco>(_dbContext));
            _provinceLogic = new App_ProvinceLogic(_cache, new EntityFrameworkGenericRepository<App_ProvincePoco>(_dbContext));
            deliveryOrderViewModel.Cities = _cityLogic.GetList();
            deliveryOrderViewModel.Provinces = _provinceLogic.GetList();

            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
            deliveryOrderViewModel.Configuration = _configurationLogic.GetList().FirstOrDefault();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            deliveryOrderViewModel.Customers = _customerLogic.GetList();

            _deliveryOptionLogic = new Lms_DeliveryOptionLogic(_cache, new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
            deliveryOrderViewModel.DeliveryOptions = _deliveryOptionLogic.GetList();

            _unitTypeLogic = new Lms_UnitTypeLogic(_cache, new EntityFrameworkGenericRepository<Lms_UnitTypePoco>(_dbContext));
            deliveryOrderViewModel.UnitTypes = _unitTypeLogic.GetList();

            _weightScaleLogic = new Lms_WeightScaleLogic(_cache, new EntityFrameworkGenericRepository<Lms_WeightScalePoco>(_dbContext));
            deliveryOrderViewModel.WeightScales = _weightScaleLogic.GetList();

            //_orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
            //deliveryOrderViewModel.OrderAdditionalServices = _orderAdditionalServiceLogic.GetList();

            _additionalServiceLogic = new Lms_AdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_AdditionalServicePoco>(_dbContext));
            deliveryOrderViewModel.AdditionalServices = _additionalServiceLogic.GetList();


            return deliveryOrderViewModel;
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