﻿using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class DeliveryOrderViewModel
    {
        public Lms_ConfigurationPoco Configuration { get; set; }
        public List<Lms_OrderAdditionalServicePoco> OrderAdditionalServices { get; set; }
        public List<Lms_CustomerPoco> Customers { get; set; }
        public List<Lms_EmployeePoco> Employees { get; set; }
        public List<Lms_AdditionalServicePoco> AdditionalServices { get; set; }
        public List<Lms_DeliveryOptionPoco> DeliveryOptions  { get; set; }
        public List<Lms_UnitTypePoco> UnitTypes  { get; set; }
        public List<Lms_WeightScalePoco> WeightScales  { get; set; }

        public List<App_CityPoco> Cities { get; set; }
        public List<App_ProvincePoco> Provinces { get; set; }

        public List<DispatchBoardDataTable> DispatchBoardData { get; set; }
    }

    public class DispatchBoardDataTable
    {
        public string OrderDateString { get; set; }
        public int DeliveryOptionId { get; set; }
        public string DeliveryOptionName { get; set; }
        public string DeliveryOptionCode { get; set; }
        public string WayBillNumber { get; set; }
        public string CustomerRefNumber { get; set; }
        public int UnitTypeId { get; set; }
        public string UnitTypeName { get; set; }
        public int UnitQuantity { get; set; }
        public string SpcIns { get; set; }
        public int ShipperCustomerId { get; set; }
        public string ShipperCustomerName { get; set; }
        public int ConsigneeCustomerId { get; set; }
        public string ConsigneeCustomerName { get; set; }
        public int BillerCustomerId { get; set; }
        public string BillerCustomerName { get; set; }
    }
}
