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
            return View(GetDataForDispatchBoard());
        }

        [HttpGet]
        public IActionResult PartialViewDataTable()
        {
            ValidateSession();
            return PartialView("_PartialViewOrderDispatchData", GetDospatchedOrders());
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
                                var orderStatus = new Lms_OrderStatusPoco();
                                orderStatus = orderStatuses.Where(c => c.OrderId == order.Id).FirstOrDefault();
                                if (orderStatus != null)
                                {
                                    orderStatus.IsDispatched = true;
                                    orderStatus.DispatchedToEmployeeId = employeeNumber;
                                    orderStatus.DispatchedDatetime = dispatchDate == null ? DateTime.Now : dispatchDate;

                                    _orderStatusLogic.Update(orderStatus);
                                }
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


        private DispatchBoardViewModel GetDataForDispatchBoard()
        {
            DispatchBoardViewModel dispatchBoardViewModel = new DispatchBoardViewModel();
            var singleOrder = _orderLogic.GetList().Where(c => c.OrderTypeId == 1).ToList();
            var pendingOrders = _orderStatusLogic.GetList().Where(c => c.IsDispatched == false || c.IsDispatched == null).ToList();

            dispatchBoardViewModel.Orders = (from order in singleOrder
                                             join pendingorder in pendingOrders on order.Id equals pendingorder.OrderId
                                             select order).ToList();

            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
            dispatchBoardViewModel.Configuration = _configurationLogic.GetList().FirstOrDefault();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            dispatchBoardViewModel.Customers = _customerLogic.GetList();

            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            if (dispatchBoardViewModel.Configuration.IsSignInRequiredForDispatch != null)
            {
                if ((bool)dispatchBoardViewModel.Configuration.IsSignInRequiredForDispatch)
                {
                    _employeeTimesheetLogic = new Lms_EmployeeTimesheetLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeTimesheetPoco>(_dbContext));
                    var employees = _employeeLogic.GetList();
                    var singedInEmployees = _employeeTimesheetLogic.GetList();

                    var filterdEmployees = (from employee in employees
                                            join signedInEmp in singedInEmployees on employee.Id equals signedInEmp.EmployeeId
                                            where signedInEmp.SignOutDatetime is null && employee.IsActive == true
                                            select employee).ToList();

                    dispatchBoardViewModel.Employees = filterdEmployees;

                }
                else
                {
                    dispatchBoardViewModel.Employees = _employeeLogic.GetList().Where(c => c.IsActive == true).ToList();
                }
            }

            return dispatchBoardViewModel;

        }

        private DispatchBoardViewModel GetDospatchedOrders()
        {
            DispatchBoardViewModel dispatchBoardViewModel = new DispatchBoardViewModel();
            dispatchBoardViewModel.Orders = _orderLogic.GetList().Where(c => c.OrderTypeId == 1).ToList();

            _configurationLogic = new Lms_ConfigurationLogic(_cache, new EntityFrameworkGenericRepository<Lms_ConfigurationPoco>(_dbContext));
            dispatchBoardViewModel.Configuration = _configurationLogic.GetList().FirstOrDefault();

            _customerLogic = new Lms_CustomerLogic(_cache, new EntityFrameworkGenericRepository<Lms_CustomerPoco>(_dbContext));
            dispatchBoardViewModel.Customers = _customerLogic.GetList();

            _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
            if (dispatchBoardViewModel.Configuration.IsSignInRequiredForDispatch != null)
            {
                if ((bool)dispatchBoardViewModel.Configuration.IsSignInRequiredForDispatch)
                {
                    _employeeTimesheetLogic = new Lms_EmployeeTimesheetLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeeTimesheetPoco>(_dbContext));
                    var employees = _employeeLogic.GetList();
                    var singedInEmployees = _employeeTimesheetLogic.GetList();

                    var filterdEmployees = (from employee in employees
                                            join signedInEmp in singedInEmployees on employee.Id equals signedInEmp.EmployeeId
                                            where signedInEmp.SignOutDatetime is null && employee.IsActive == true
                                            select employee).ToList();

                    dispatchBoardViewModel.Employees = filterdEmployees;

                }
                else
                {
                    dispatchBoardViewModel.Employees = _employeeLogic.GetList().Where(c => c.IsActive == true).ToList();
                }
            }

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