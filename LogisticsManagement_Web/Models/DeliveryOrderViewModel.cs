using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class DeliveryOrderViewModel
    {
        public Lms_ConfigurationPoco Configuration { get; set; }
        public List<Lms_OrderPoco> Orders { get; set; }
        public List<Lms_OrderAdditionalServicePoco> OrderAdditionalServices { get; set; }
        public List<Lms_CustomerPoco> Customers { get; set; }
        public List<Lms_AdditionalServicePoco> AdditionalServices { get; set; }
        public List<Lms_DeliveryOptionPoco> DeliveryOptions  { get; set; }
        public List<Lms_UnitTypePoco> UnitTypes  { get; set; }
        public List<Lms_WeightScalePoco> WeightScales  { get; set; }

        public List<App_CityPoco> Cities { get; set; }
        public List<App_ProvincePoco> Provinces { get; set; }
    }
}
