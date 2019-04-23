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
    public class OrderPickupController : Controller
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

        public OrderPickupController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();
            return View();
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewOrderPickedupData", GetPickedupOrders().Where(c => c.IsOrderPickedup == true && c.IsOrderPassedOn == null && c.IsOrderDelivered == null));
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
                    var wayBillNumber = Convert.ToString(orderData[0]);
                    var waitTime = string.IsNullOrEmpty(Convert.ToString(orderData[1])) == true ? null : Convert.ToDecimal(orderData[1]);
                    var pickupDate = Convert.ToDateTime(orderData[2]);

                    var orders = _orderLogic.GetList().Where(c => c.WayBillNumber == wayBillNumber).ToList();
                    var orderStatuses = _orderStatusLogic.GetList();

                    orderStatuses = (from orderStatus in orderStatuses
                                     join order in orders on orderStatus.OrderId equals order.Id
                                     select orderStatus).ToList();

                    using (var scope = new TransactionScope())
                    {
                        foreach (var orderStatus in orderStatuses)
                        {
                            orderStatus.IsPickedup = true;
                            orderStatus.PickupWaitTimeHour = waitTime;
                            orderStatus.PickupDatetime = pickupDate == null ? DateTime.Now : pickupDate;

                            _orderStatusLogic.Update(orderStatus);
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
            bool result = false;
            try
            {
                var orders = _orderLogic.GetList().Where(c => c.WayBillNumber == id).ToList();
                var dispatchedList = _orderStatusLogic.GetList();

                dispatchedList = (from dispatch in dispatchedList
                                  join order in orders on dispatch.OrderId equals order.Id
                                  select dispatch).ToList();

                using (var scope = new TransactionScope())
                {
                    foreach (var item in dispatchedList)
                    {
                        item.IsPickedup = null;
                        item.PickupDatetime = null;
                        item.PickupWaitTimeHour = null;

                        _orderStatusLogic.Update(item);
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

        private List<DispatchedOrderViewModel> GetPickedupOrders()
        {
            List<DispatchedOrderViewModel> dispatchedOrderViewModels = new List<DispatchedOrderViewModel>();

            var orderList = _orderLogic.GetList().Where(c => c.OrderTypeId == 1).ToList();
            var dispatchedList = _orderStatusLogic.GetList();

            dispatchedList = (from dispatch in dispatchedList
                              join order in orderList on dispatch.OrderId equals order.Id
                              select dispatch).ToList();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            var customerList = _customerLogic.GetList();

            _addressLogic = new Lms_AddressLogic(_cache, new EntityFrameworkGenericRepository<Lms_AddressPoco>(_dbContext));
            var addressList = _addressLogic.GetList();

            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            var employeeList = _employeeLogic.GetList();

            foreach (var item in dispatchedList)
            {
                DispatchedOrderViewModel dispatchedOrderViewModel = new DispatchedOrderViewModel();
                dispatchedOrderViewModel.OrderId = item.OrderId;
                dispatchedOrderViewModel.WayBillNumber = orderList.Where(c => c.Id == item.OrderId).FirstOrDefault().WayBillNumber;
                dispatchedOrderViewModel.ShipperId = (int)orderList.Where(c => c.Id == item.OrderId).FirstOrDefault().ShipperCustomerId;
                dispatchedOrderViewModel.ShipperName = customerList.Where(c => c.Id == dispatchedOrderViewModel.ShipperId).FirstOrDefault().CustomerName;

                var shippMailingId = customerList.Where(d => d.Id == dispatchedOrderViewModel.ShipperId).FirstOrDefault().MailingAddressId;
                if (shippMailingId == null)
                {
                    shippMailingId = customerList.Where(d => d.Id == dispatchedOrderViewModel.ShipperId).FirstOrDefault().BillingAddressId;
                }

                var shippAddress = addressList.Where(c => c.Id == shippMailingId).FirstOrDefault();
                dispatchedOrderViewModel.ShipperAddress = shippAddress.UnitNumber + " " + shippAddress.AddressLine + "  " + shippAddress.PrimaryPhoneNumber;


                dispatchedOrderViewModel.ConsigneeId = (int)orderList.Where(c => c.Id == item.OrderId).FirstOrDefault().ConsigneeCustomerId;
                dispatchedOrderViewModel.ConsigneeName = customerList.Where(c => c.Id == dispatchedOrderViewModel.ConsigneeId).FirstOrDefault().CustomerName;

                var consigMailingId = customerList.Where(d => d.Id == dispatchedOrderViewModel.ConsigneeId).FirstOrDefault().MailingAddressId;
                if (consigMailingId == null)
                {
                    consigMailingId = customerList.Where(d => d.Id == dispatchedOrderViewModel.ConsigneeId).FirstOrDefault().BillingAddressId;
                }

                var consigAddress = addressList.Where(c => c.Id == consigMailingId).FirstOrDefault();
                dispatchedOrderViewModel.ConsigneeAddress = consigAddress.UnitNumber + " " + consigAddress.AddressLine + "  " + consigAddress.PrimaryPhoneNumber;

                dispatchedOrderViewModel.EmployeeId = item.DispatchedToEmployeeId;

                if (item.DispatchedToEmployeeId != null)
                {
                    var employee = employeeList.Where(c => c.Id == dispatchedOrderViewModel.EmployeeId).FirstOrDefault();
                    dispatchedOrderViewModel.EmployeeName = employee.FirstName + "  " + employee.LastName;
                }

                dispatchedOrderViewModel.IsOrderDispatched = item.IsDispatched;
                dispatchedOrderViewModel.IsOrderPickedup = item.IsPickedup;
                dispatchedOrderViewModel.IsOrderPassedOn = item.IsPassedOff;
                dispatchedOrderViewModel.IsOrderDelivered = item.IsDelivered;

                dispatchedOrderViewModel.DispatchDatetime = item.DispatchedDatetime;
                dispatchedOrderViewModel.PickupDatetime = item.PickupDatetime;
                dispatchedOrderViewModel.PassOffDatetime = item.PassOffDatetime;
                dispatchedOrderViewModel.DeliverDatetime = item.DeliveredDatetime;

                dispatchedOrderViewModels.Add(dispatchedOrderViewModel);
            }

            return dispatchedOrderViewModels;

        }

        public JsonResult GetOrderByWayBillId(string id)
        {
            try
            {
                var orderPocos = _orderLogic.GetList().Where(c => c.WayBillNumber == id).ToList();

                _orderAdditionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));
                var orderStatuses = _orderStatusLogic.GetList(); //.Where(c => orderPocos.Select(d => d.Id).ToList().Contains(c.OrderId)).ToList();

                var orderDetails = (from orderStatus in orderStatuses
                                    join order in orderPocos on orderStatus.OrderId equals order.Id
                                    where order.OrderTypeId == 1
                                    select orderStatus).FirstOrDefault();

                _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
                var employee = _employeeLogic.GetList().Where(c => c.Id == orderDetails.DispatchedToEmployeeId).FirstOrDefault();

                DispatchedOrderViewModel dispatchedOrderViewModel = new DispatchedOrderViewModel();
                dispatchedOrderViewModel.EmployeeId = employee.Id;
                dispatchedOrderViewModel.EmployeeName = employee.FirstName + "  " + employee.LastName;
                dispatchedOrderViewModel.DispatchDatetime = orderDetails.DispatchedDatetime;
                dispatchedOrderViewModel.IsOrderDispatched = orderDetails.IsDispatched;
                dispatchedOrderViewModel.IsOrderPickedup = orderDetails.IsPickedup;
                dispatchedOrderViewModel.IsOrderPassedOn = orderDetails.IsPassedOff;
                dispatchedOrderViewModel.IsOrderDelivered = orderDetails.IsDelivered;

                return Json(JsonConvert.SerializeObject(dispatchedOrderViewModel));

            }
            catch (Exception ex)
            {
                return Json("");
            }
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