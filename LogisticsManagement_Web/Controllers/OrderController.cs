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

        private DeliveryOrderViewModel GetOrderData()
        {
            DeliveryOrderViewModel deliveryOrderViewModel = new DeliveryOrderViewModel();
            deliveryOrderViewModel.Orders = _orderLogic.GetList();

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
                    List<Lms_OrderAdditionalServicePoco> orderAdditionalServices = JsonConvert.DeserializeObject<List<Lms_OrderAdditionalServicePoco>>(JsonConvert.SerializeObject(orderData[0]));

                    if (orderPoco.Id < 1 && orderPoco.BillToCustomerId > 0 && orderPoco.ShipperCustomerId > 0 && orderPoco.ConsigneeCustomerId > 0)
                    {
                        orderPoco.CreatedBy = sessionData.UserId;
                        var orderInfo = _orderLogic.CreateNewOrder(orderPoco, orderAdditionalServices);
                        if (!string.IsNullOrEmpty(orderInfo))
                        {
                            var jObject = JObject.Parse(orderInfo);
                            var returnedObject = (string)jObject.SelectToken("ReturnedValue");
                            result = (string)JObject.Parse(returnedObject).SelectToken("OrderId");
                            if (result.Length > 0)
                            {
                                result = Convert.ToInt32(result) < 1 ? "" : result;
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
                    List<Lms_OrderAdditionalServicePoco> orderAdditionalServices = JsonConvert.DeserializeObject<List<Lms_OrderAdditionalServicePoco>>(JsonConvert.SerializeObject(orderData[0]));

                    if (orderPoco.Id > 0 && orderPoco.ShipperCustomerId > 0 && orderPoco.ConsigneeCustomerId > 0)
                    {
                        var order = _orderLogic.GetSingleById(orderPoco.Id);
                        // it is required to pull existing data first, 
                        // cause there are some data which do not come from UI

                        order.ReferenceNumber = orderPoco.ReferenceNumber;
                        order.CargoCtlNumber = orderPoco.CargoCtlNumber;
                        order.AwbCtnNumber = orderPoco.CargoCtlNumber;
                        order.ShipperCustomerId = orderPoco.ShipperCustomerId;
                        order.ConsigneeCustomerId = orderPoco.ConsigneeCustomerId;
                        order.BillToCustomerId = orderPoco.BillToCustomerId;
                        order.ScheduledPickupDate = orderPoco.ScheduledPickupDate;

                        order.CityId = orderPoco.CityId;
                        order.DeliveryOptionId = orderPoco.DeliveryOptionId;
                        order.VehicleTypeId = orderPoco.VehicleTypeId;
                        order.WeightScaleId = orderPoco.WeightScaleId;
                        order.WeightTotal = orderPoco.WeightTotal;
                        order.UnitQuantity = orderPoco.UnitQuantity;
                        order.OrderBasicCost = orderPoco.OrderBasicCost;
                        order.BasicCostOverriden = orderPoco.BasicCostOverriden;
                        order.FuelSurchargePercentage = orderPoco.FuelSurchargePercentage;
                        order.DiscountPercentOnOrderCost = orderPoco.DiscountPercentOnOrderCost;
                        order.ApplicableGstPercent = orderPoco.ApplicableGstPercent;
                        order.TotalOrderCost = orderPoco.TotalOrderCost;
                        order.TotalAdditionalServiceCost = orderPoco.TotalAdditionalServiceCost;
                        order.OrderedBy = orderPoco.OrderedBy;
                        order.ContactName = orderPoco.ContactName;
                        order.ContactPhoneNumber = orderPoco.ContactPhoneNumber;
                        order.Remarks = orderPoco.Remarks;


                        _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));

                        var orderServices = _orderAdditionalServiceLogic.GetList().Where(c => c.OrderId == orderPoco.Id).ToList();
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

                        var poco = _orderLogic.Update(order);

                        result = poco.Id.ToString();
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

                    var poco = _orderLogic.GetSingleById(Convert.ToInt32(id));

                    _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));

                    var orderServices = _orderAdditionalServiceLogic.GetList().Where(c => c.OrderId == poco.Id).ToList();
                    if (orderServices.Count > 0)
                    {
                        foreach (var item in orderServices)
                        {
                            _orderAdditionalServiceLogic.Remove(item);
                        }
                    }
                    _orderLogic.Remove(poco);

                    scope.Complete();

                    result = true;
                }

            }
            catch (Exception ex)
            {

            }
            return Json(result);
        }


        public JsonResult GetOrderById(string id)
        {
            var orderPoco = _orderLogic.GetList().Where(c => c.Id == Convert.ToInt32(id));
            var orderAdditionalServices = _orderAdditionalServiceLogic.GetList().Where(c => c.OrderId == Convert.ToInt32(id)).ToList();
            return Json(JsonConvert.SerializeObject(new { order = orderPoco, additionalServices = new[] {orderAdditionalServices}}));
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