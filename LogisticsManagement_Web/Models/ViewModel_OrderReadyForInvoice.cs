using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_OrderReadyForInvoice
    {
        public int OrderId { get; set; }
        public string WayBillNumber { get; set; }
        public int OrderTypeId { get; set; }
        public string OrderType { get; set; }
        public string CustomerReferenceNo { get; set; }
        public DateTime WaybillDate { get; set; }
        public int ShipperId { get; set; }
        public string ShipperName { get; set; }
        public int ConsigneeId { get; set; }
        public string ConsigneeName { get; set; }
        public int UnitTypeId { get; set; }
        public string UnitTypeShortCode { get; set; }
        public string UnitTypeName { get; set; }
        public int? UnitQty { get; set; }
        public int? SkidQty { get; set; }
        public int? WeightScaleId { get; set; }
        public string WeightShortName { get; set; }
        public decimal? WeightTotal { get; set; }
        public int BillToCustomerId { get; set; }
        public string BillerName { get; set; }
        public string  BillerEmail { get; set; }
        public string OrderedBy { get; set; }
        public decimal TotalOrderCost { get; set; }
        public decimal TotalReturnOrderCost { get; set; }
    }
}
