using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_InvoiceWiseWayBill
    {
        public string WaybillNumber { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal TotalWaybillAmount { get; set; }
        public decimal? TotalTaxAmount { get; set; }
        /// <summary>
        /// Whether the waybill is fully paid / waived; means no dues left.
        /// </summary>
        public bool IsCleared { get; set; }
    }
}
