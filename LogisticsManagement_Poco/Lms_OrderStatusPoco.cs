﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_OrderStatus")]
    public class Lms_OrderStatusPoco : IPoco
    {
        [Key]
        [Column("OrderStatusId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Lms_OrderPoco")]
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public bool? IsDispatched { get; set; }
        public int? DispatchedToEmployeeId { get; set; }
        public int? VehicleId { get; set; }
        public DateTime? DispatchedDatetime { get; set; }
        public bool? IsPickedup { get; set; }
        public DateTime? PickupDatetime { get; set; }
        public decimal? PickupWaitTimeHour { get; set; }
        public bool? IsPassedOff { get; set; }
        public int? PassedOffFromEmployeeId { get; set; }
        public int? PassedOffToEmployeeId { get; set; }
        public DateTime? PassOffDatetime { get; set; }
        public decimal? PassOffWaitTimeHour { get; set; }
        public bool? IsDelivered { get; set; }
        public DateTime? DeliveredDatetime { get; set; }
        public decimal? DeliveryWaitTimeHour { get; set; }
        public string ProofOfDeliveryNote { get; set; }
        public string ReceivedByName { get; set; }
        public byte[] ReceivedBySignature { get; set; }
        public DateTime StatusLastUpdatedOn { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

        public Lms_OrderPoco Lms_OrderPoco { get; set; }
    }
}
