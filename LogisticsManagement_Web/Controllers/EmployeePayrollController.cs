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
    public class EmployeePayrollController : Controller
    {
        private Lms_EmployeePayrollLogic _employeePayrollLogic;
        private Lms_OrderLogic _orderLogic;
        private Lms_OrderStatusLogic _orderStatusLogic;
        private Lms_OrderAdditionalServiceLogic _additionalServiceLogic;
        private Lms_DeliveryOptionLogic _deliveryOptionLogic;
        private Lms_EmployeeLogic _employeeLogic;


        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;
        SessionData sessionData = new SessionData();

        public EmployeePayrollController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _employeePayrollLogic = new Lms_EmployeePayrollLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePayrollPoco>(_dbContext));
        }

        public IActionResult Index()
        {
            ValidateSession();

            var employeeController = new EmployeeController(_cache, _dbContext);
            var employeeViewModel = employeeController.GetEmployeeData();

            return View(employeeViewModel);
        }


        [HttpGet]
        public IActionResult PartialLoadEmployees(string empTypeId)
        {

            var employeeViewModel = new ViewModel_Employee();

            if (empTypeId != "")
            {
                var employeeController = new EmployeeController(_cache, _dbContext);
                employeeViewModel = employeeController.GetEmployeeData();
                if (Convert.ToInt32(empTypeId) > 0)
                {
                    employeeViewModel.Employees = employeeViewModel.Employees.Where(c => c.EmployeeTypeId == Convert.ToInt32(empTypeId)).ToList();
                }
            }

            return PartialView("_PartialViewEmployeeList", employeeViewModel);

        }

        [HttpGet]
        public IActionResult PartialLoadEmployeeOrders(string empId, string fromDate, string toDate)
        {
            List<ViewModel_PayrollOrder> viewModelPayrollOrders = new List<ViewModel_PayrollOrder>();

            if (empId != "")
            {
                int selectedEmpId = Convert.ToInt16(empId);
                DateTime selectedFromDate = Convert.ToDateTime(fromDate);
                DateTime selectedToDate = Convert.ToDateTime(toDate);

                _deliveryOptionLogic = new Lms_DeliveryOptionLogic(_cache, new EntityFrameworkGenericRepository<Lms_DeliveryOptionPoco>(_dbContext));
                var deliveryOptionList = _deliveryOptionLogic.GetList();

                _employeeLogic = new Lms_EmployeeLogic(_cache, new EntityFrameworkGenericRepository<Lms_EmployeePoco>(_dbContext));
                var employeeList = _employeeLogic.GetList();

                _additionalServiceLogic = new Lms_OrderAdditionalServiceLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderAdditionalServicePoco>(_dbContext));

                _orderLogic = new Lms_OrderLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderPoco>(_dbContext));
                var orderList = _orderLogic.GetList();

                _orderStatusLogic = new Lms_OrderStatusLogic(_cache, new EntityFrameworkGenericRepository<Lms_OrderStatusPoco>(_dbContext));
                var orderStatusList = _orderStatusLogic.GetList().Where(c => (c.DispatchedToEmployeeId == selectedEmpId || c.PassedOffToEmployeeId == selectedEmpId) && c.CreateDate >= selectedFromDate && c.CreateDate <= selectedToDate && c.IsDelivered == true).ToList();

                orderList = (from order in orderList
                             join status in orderStatusList on order.Id equals status.OrderId
                             select order).ToList();

                foreach (var order in orderList)
                {
                    ViewModel_PayrollOrder payrollOrder = new ViewModel_PayrollOrder();
                    var empInfo = _employeeLogic.GetSingleById(selectedEmpId);

                    var orderStatus = orderStatusList.Where(c => c.OrderId == order.Id).FirstOrDefault();


                    payrollOrder.WaybillNumber = order.WayBillNumber;
                    payrollOrder.WaybillDate = order.CreateDate;
                    payrollOrder.OrderType = order.OrderTypeId == 1 ? "Single" : "Return";
                    payrollOrder.DeliveryOptionShortCode = deliveryOptionList.Where(c=>c.Id == order.DeliveryOptionId).FirstOrDefault().ShortCode;
                    payrollOrder.PickupEmployeeId = (int)orderStatus.DispatchedToEmployeeId;
                    payrollOrder.PickupEmployeeName = employeeList.Where(c=>c.Id == payrollOrder.PickupEmployeeId).FirstOrDefault().FirstName;

                    var deliveryEmpId = orderStatusList.Where(c => c.OrderId == order.Id).FirstOrDefault().PassedOffToEmployeeId;
                    if (deliveryEmpId != null && deliveryEmpId > 0) {
                        payrollOrder.DeliveryEmployeeId = (int)orderStatus.PassedOffToEmployeeId;
                        payrollOrder.DeliveryEmployeeName = employeeList.Where(c => c.Id == payrollOrder.DeliveryEmployeeId).FirstOrDefault().FirstName;
                    }
                    else {
                        payrollOrder.DeliveryEmployeeId = payrollOrder.PickupEmployeeId;
                        payrollOrder.DeliveryEmployeeName = payrollOrder.PickupEmployeeName;
                    }

                    payrollOrder.WaybillBaseAmount = order.OrderBasicCost;
                    if (order.BasicCostOverriden != null && order.BasicCostOverriden > 0) {
                        payrollOrder.WaybillBaseAmount = (decimal)order.BasicCostOverriden;
                    }

                    payrollOrder.OrderCommissionPercent = empInfo.CommissionPercentage == null ? 0 : (decimal)empInfo.CommissionPercentage;
                    payrollOrder.OrderFuelPercent = empInfo.FuelPercentage == null ? 0 : (decimal)empInfo.FuelPercentage;

                    payrollOrder.OrderCommissionAmnt = payrollOrder.WaybillBaseAmount * payrollOrder.OrderCommissionPercent / 100;
                    payrollOrder.OrderFuelAmnt = payrollOrder.WaybillBaseAmount * payrollOrder.OrderFuelPercent / 100;

                    payrollOrder.AddServiceAmnt = 0;

                    var additionalServices = _additionalServiceLogic.GetList().Where(c=>c.OrderId == order.Id);
                    foreach (var addSrv in additionalServices) {
                        if (addSrv.DriverPercentageOnAddService != null && addSrv.DriverPercentageOnAddService > 0)
                        {
                            payrollOrder.AddServiceAmnt = payrollOrder.AddServiceAmnt + (addSrv.AdditionalServiceFee * (decimal)addSrv.DriverPercentageOnAddService / 100);
                        }
                    }
                    payrollOrder.WaitTime = orderStatus.DeliveryWaitTimeHour;

                    viewModelPayrollOrders.Add(payrollOrder);
                }
            }

            return PartialView("_PartialViewEmployeeDeliveries", viewModelPayrollOrders);

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