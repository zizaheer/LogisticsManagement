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
    public class OrderDispatchController : Controller
    {
        //private IMemoryCache _cache;  // To do later 

        private Lms_OrderLogic _orderLogic;
        private Lms_OrderStatusLogic _orderStatusLogic;
        private Lms_CustomerLogic _customerLogic;
        private Lms_AddressLogic _addressLogic;
        private Lms_ConfigurationLogic _configurationLogic;
        private Lms_EmployeeLogic _employeeLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public OrderDispatchController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
            _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();
            return View(GetPendingDispatchData());
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewOrderDispatchData", GetDispatchedOrders().Where(c => c.IsOrderDispatched == true && c.IsOrderPickedup == null));
        }

        [HttpGet]
        public IActionResult PartialPendingDispatchDataTable()
        {
            ValidateSession();
            return PartialView("_PartialPendingDispatchData", GetPendingDispatchData());
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
                    var wayBillNumberList = JArray.Parse(JsonConvert.SerializeObject(orderData[0]));
                    var employeeNumber = Convert.ToInt32(orderData[1]);
                    var dispatchDate = Convert.ToDateTime(orderData[2]);

                    var orders = _orderLogic.GetList();
                    var orderStatuses = _orderStatusLogic.GetList();

                    using (var scope = new TransactionScope())
                    {
                        foreach (var item in wayBillNumberList)
                        {
                            var wbNumber = item.SelectToken("wbillNumber").ToString();

                            orders = orders.Where(c => c.WayBillNumber == wbNumber).ToList();
                            foreach (var order in orders)
                            {
                                var status = orderStatuses.Where(c => c.OrderId == order.Id).FirstOrDefault();

                                status.IsDispatched = true;
                                status.DispatchedToEmployeeId = employeeNumber;
                                status.DispatchedDatetime = dispatchDate == null ? DateTime.Now : dispatchDate;
                                status.StatusLastUpdatedOn = DateTime.Now;

                                _orderStatusLogic.Update(status);
                            }

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
                        item.IsDispatched = null;
                        item.DispatchedDatetime = null;
                        item.DispatchedToEmployeeId = null;

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


        private List<DispatchedOrderViewModel> GetDispatchedOrders()
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

                dispatchedOrderViewModel.IsOrderDispatched = item.IsDispatched;
                dispatchedOrderViewModel.DispatchDatetime = item.DispatchedDatetime;
                dispatchedOrderViewModel.DispatchedEmployeeId = item.DispatchedToEmployeeId;
                if (item.DispatchedToEmployeeId != null)
                {
                    var employee = employeeList.Where(c => c.Id == dispatchedOrderViewModel.DispatchedEmployeeId).FirstOrDefault();
                    dispatchedOrderViewModel.DispatchedEmployeeName = employee.FirstName + "  " + employee.LastName;
                    if (!string.IsNullOrEmpty(employee.MobileNumber))
                    {
                        dispatchedOrderViewModel.DispatchedEmployeePhone = employee.MobileNumber;
                    }
                    else if (!string.IsNullOrEmpty(employee.PhoneNumber))
                    {
                        dispatchedOrderViewModel.DispatchedEmployeePhone = employee.PhoneNumber;
                    }
                }

               
                dispatchedOrderViewModel.IsOrderPickedup = item.IsPickedup;
                dispatchedOrderViewModel.PickupDatetime = item.PickupDatetime;

                dispatchedOrderViewModel.PassOnDatetime = item.PassOffDatetime;
                dispatchedOrderViewModel.IsOrderPassedOn = item.IsPassedOff;

                dispatchedOrderViewModel.IsOrderDelivered = item.IsDelivered;
                dispatchedOrderViewModel.DeliverDatetime = item.DeliveredDatetime;


                dispatchedOrderViewModels.Add(dispatchedOrderViewModel);

            }

            return dispatchedOrderViewModels;

        }

        private DispatchBoardViewModel GetPendingDispatchData()
        {
            DispatchBoardViewModel dispatchBoardViewModel = new DispatchBoardViewModel();

            dispatchBoardViewModel.DispatchedOrderViewModels = GetDispatchedOrders().Where(c => c.IsOrderDispatched == null).ToList();
            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            dispatchBoardViewModel.Employees = _employeeLogic.GetList();

            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
            dispatchBoardViewModel.Configuration = _configurationLogic.GetList().FirstOrDefault();

            return dispatchBoardViewModel;
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