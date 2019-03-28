using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_Poco;

namespace LogisticsManagement_Web.Models
{
    public class TariffViewModel
    {
        public Lms_TariffPoco Tariff { get; set; }
        public List<App_CityPoco> Cities { get; set; }
        public List<Lms_DeliveryOptionPoco> DeliveryOptions { get; set; }
        public List<Lms_VehicleTypePoco> VehicleTypes { get; set; }
        public List<Lms_UnitTypePoco> UnitTypes { get; set; }
        public List<Lms_TariffPoco> Tariffs { get; set; }
        public List<Lms_WeightScalePoco> WeightScales { get; set; }
        public List<Lms_VehicleUnitMappingPoco> VehicleUnitMappings { get; set; }
    }
}
