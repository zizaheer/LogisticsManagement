﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LogisticsManagement_Poco
{
    [Table("Lms_Tariff")]
    public class Lms_TariffPoco : IPoco
    {
        [Key]
        [Column("TariffId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CityId { get; set; }
        public int? DeliveryOptionId { get; set; }
        public int? VehicleTypeId { get; set; }
        public int? UnitTypeId { get; set; }
        public int? WeightScaleId { get; set; }
        public decimal? UptoWeight { get; set; }
        public decimal? FirstUnitPrice { get; set; }
        public decimal? PerUnitPrice { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
