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
    public class OrderDeliverController : Controller
    {
        //private IMemoryCache _cache;  // To do later 

        private Lms_OrderLogic _orderLogic;
        private Lms_OrderStatusLogic _orderStatusLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_EmployeeLogic _employeeLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public OrderDeliverController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();
            return View(GetEmployees());
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewOrderDeliveredData", GetDeliveredOrders().Where(c => c.IsOrderDelivered == true));
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
                    var deliveryDate = Convert.ToDateTime(orderData[2]);
                    var proofNote = Convert.ToString(orderData[3]);
                    var receivedByName = Convert.ToString(orderData[4]);
                    var receivedBySign = Convert.ToString(orderData[5]);
                    var orderTypeId = Convert.ToInt16(orderData[6]);

                    byte[] imageByte = null;
                    if (receivedBySign != null && receivedBySign != "")
                    {
                        var base64Signature = receivedBySign.Split(",")[1];
                        imageByte = string.IsNullOrEmpty(base64Signature) == true ? null : Convert.FromBase64String(base64Signature);
                    }

                    var order = _orderLogic.GetList().Where(c => c.WayBillNumber == wayBillNumber && c.OrderTypeId == orderTypeId).FirstOrDefault();
                    var orderStatus = _orderStatusLogic.GetList().Where(c => c.OrderId == order.Id).FirstOrDefault();

                    if (orderStatus != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            orderStatus.IsDelivered = true;
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
        public IActionResult Remove(string id)
        {
            ValidateSession();

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
                    foreach (var item in dispatchedList)
                    {
                        item.IsDelivered = null;
                        item.DeliveredDatetime = null;
                        item.DeliveryWaitTimeHour = null;
                        item.ReceivedByName = null;
                        item.ReceivedBySignature = null;
                        item.ProofOfDeliveryNote = null;

                        item.StatusLastUpdatedOn = DateTime.Now;

                        _orderStatusLogic.Update(item);
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

        private List<DispatchedOrderViewModel> GetDeliveredOrders()
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

                dispatchedOrderViewModel.DispatchedEmployeeId = item.DispatchedToEmployeeId;

                if (item.DispatchedToEmployeeId != null)
                {
                    var employee = employeeList.Where(c => c.Id == dispatchedOrderViewModel.DispatchedEmployeeId).FirstOrDefault();
                    dispatchedOrderViewModel.DispatchedEmployeeName = employee.FirstName + "  " + employee.LastName;
                }

                dispatchedOrderViewModel.IsOrderDispatched = item.IsDispatched;
                dispatchedOrderViewModel.DispatchDatetime = item.DispatchedDatetime;

                dispatchedOrderViewModel.IsOrderPickedup = item.IsPickedup;
                dispatchedOrderViewModel.PickupDatetime = item.PickupDatetime;

                dispatchedOrderViewModel.IsOrderPassedOn = item.IsPassedOff;
                dispatchedOrderViewModel.PassOnDatetime = item.PassOffDatetime;
                dispatchedOrderViewModel.PassOnEmployeeId = item.PassedOffToEmployeeId;
                if (item.PassedOffToEmployeeId != null)
                {
                    var employee = employeeList.Where(c => c.Id == dispatchedOrderViewModel.PassOnEmployeeId).FirstOrDefault();
                    dispatchedOrderViewModel.PassOnEmployeeName = employee.FirstName + "  " + employee.LastName;
                    if (!string.IsNullOrEmpty(employee.MobileNumber))
                    {
                        dispatchedOrderViewModel.PassOnEmployeePhone = employee.MobileNumber;
                    }
                    else if (!string.IsNullOrEmpty(employee.PhoneNumber))
                    {
                        dispatchedOrderViewModel.PassOnEmployeePhone = employee.PhoneNumber;
                    }
                }
                dispatchedOrderViewModel.PassOnWaitTime = item.PassOffWaitTimeHour;


                dispatchedOrderViewModel.IsOrderDelivered = item.IsDelivered;
                dispatchedOrderViewModel.DeliverDatetime = item.DeliveredDatetime;
                dispatchedOrderViewModel.DeliveryWaitTimeInHour = item.DeliveryWaitTimeHour;
                dispatchedOrderViewModel.ReceivedByName = item.ReceivedByName;

                if (item.ReceivedBySignature != null)
                {
                    dispatchedOrderViewModel.ReceivedBySignature = Convert.ToBase64String(item.ReceivedBySignature);
                }

                dispatchedOrderViewModel.ProofOfDeliveryNote = item.ProofOfDeliveryNote;


                dispatchedOrderViewModels.Add(dispatchedOrderViewModel);
            }

            return dispatchedOrderViewModels;

        }

        private DispatchBoardViewModel GetEmployees()
        {
            DispatchBoardViewModel dispatchBoardViewModel = new DispatchBoardViewModel();

            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            dispatchBoardViewModel.Employees = _employeeLogic.GetList();

            return dispatchBoardViewModel;
        }

        public JsonResult GetOrderByWayBillId(string id)
        {
            ValidateSession();

            try
            {
                var orderPocos = _orderLogic.GetList().Where(c => c.WayBillNumber == id).ToList();
                var orderStatuses = _orderStatusLogic.GetList();

                var orderDetails = (from orderStatus in orderStatuses
                                    join order in orderPocos on orderStatus.OrderId equals order.Id
                                    where order.OrderTypeId == 1
                                    select orderStatus).FirstOrDefault();

                _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
                var employee = _employeeLogic.GetList().Where(c => c.Id == orderDetails.DispatchedToEmployeeId).FirstOrDefault();

                DispatchedOrderViewModel dispatchedOrderViewModel = new DispatchedOrderViewModel();
                dispatchedOrderViewModel.DispatchedEmployeeId = employee.Id;
                dispatchedOrderViewModel.DispatchedEmployeeName = employee.FirstName + "  " + employee.LastName;
                dispatchedOrderViewModel.DispatchDatetime = orderDetails.DispatchedDatetime;
                dispatchedOrderViewModel.IsOrderDispatched = orderDetails.IsDispatched;

                dispatchedOrderViewModel.IsOrderPickedup = orderDetails.IsPickedup;
                dispatchedOrderViewModel.PickupDatetime = orderDetails.PickupDatetime;

                dispatchedOrderViewModel.IsOrderPassedOn = orderDetails.IsPassedOff;
                dispatchedOrderViewModel.PassOnDatetime = orderDetails.PassOffDatetime;
                dispatchedOrderViewModel.PassOnEmployeeId = orderDetails.PassedOffToEmployeeId;
                dispatchedOrderViewModel.PassOnWaitTime = orderDetails.PassOffWaitTimeHour;

                dispatchedOrderViewModel.IsOrderDelivered = orderDetails.IsDelivered;
                dispatchedOrderViewModel.DeliverDatetime = orderDetails.DeliveredDatetime;
                dispatchedOrderViewModel.DeliveryWaitTimeInHour = orderDetails.DeliveryWaitTimeHour;
                dispatchedOrderViewModel.ReceivedByName = orderDetails.ReceivedByName;
                if (orderDetails.ReceivedBySignature != null)
                {
                    dispatchedOrderViewModel.ReceivedBySignature = Convert.ToBase64String(orderDetails.ReceivedBySignature);
                }

                dispatchedOrderViewModel.ProofOfDeliveryNote = orderDetails.ProofOfDeliveryNote;


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