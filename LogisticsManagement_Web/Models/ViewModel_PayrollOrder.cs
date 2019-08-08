using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_PayrollOrder
    {
        public string WaybillNumber { get; set; }
        public string OrderType { get; set; }
        public DateTime WaybillDate { get; set; }
        public string DeliveryOptionShortCode { get; set; }
        public int PickupEmployeeId { get; set; }
        public string PickupEmployeeName { get; set; }
        public int DeliveryEmployeeId { get; set; }
        public string DeliveryEmployeeName { get; set; }
        public decimal WaybillBaseAmount { get; set; }
        public decimal OrderCommissionPercent { get; set; }
        public decimal OrderFuelPercent { get; set; }
        public decimal OrderCommissionAmnt { get; set; }
        public decimal OrderFuelAmnt { get; set; }
        public decimal AddServicePercent { get; set; }
        public decimal AddServiceAmnt { get; set; }
        public decimal? WaitTime { get; set; }
    }
}
