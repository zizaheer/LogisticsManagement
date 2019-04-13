using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;
using System.Data;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_OrderLogic : BaseLogic<Lms_OrderPoco>
    {
        IMemoryCache _cache;
        public Lms_OrderLogic(IMemoryCache cache, IDataRepository<Lms_OrderPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_OrderPoco> GetList()
        {
            List<Lms_OrderPoco> _orders;
            if (!_cache.TryGetValue(App_CacheKeys.Orders, out _orders))
            {
                _orders = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Orders, _orders, cacheEntryOptions);
            }

            return _orders;
        }

        public override List<Lms_OrderPoco> GetListById(int id)
        {
            List<Lms_OrderPoco> _orders;
            if (!_cache.TryGetValue(App_CacheKeys.Orders, out _orders))
            {
                _orders = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Orders, _orders, cacheEntryOptions);
            }

            return _orders;
        }

        public override Lms_OrderPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_OrderPoco Add(Lms_OrderPoco poco)
        {
            _cache.Remove(App_CacheKeys.Employees);
            return base.Add(poco);
        }

        public override Lms_OrderPoco Update(Lms_OrderPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            _cache.Remove(App_CacheKeys.Employees);
            return base.Update(poco);
        }

        public override void Remove(Lms_OrderPoco poco)
        {
            _cache.Remove(App_CacheKeys.Employees);
            base.Remove(poco);
        }

        public override void Add(Lms_OrderPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_OrderPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_OrderPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion

        public string CreateNewOrder(Lms_OrderPoco orderPoco, List<Lms_OrderAdditionalServicePoco> orderAdditionalServices)
        {

            DataTable additionalServiceList = new DataTable();
            additionalServiceList.Columns.Add("ServiceId", typeof(int));
            additionalServiceList.Columns.Add("ServiceFee", typeof(decimal));
            additionalServiceList.Columns.Add("DriverPercentage", typeof(decimal));
            additionalServiceList.Columns.Add("IsTaxApplicable", typeof(bool));
            additionalServiceList.Columns.Add("TaxAmount", typeof(decimal));

            if (orderAdditionalServices.Count > 0)
            {
                foreach(var item in orderAdditionalServices)
                {
                    var row = additionalServiceList.NewRow();
                    row[0] = item.AdditionalServiceId;
                    row[1] = item.AdditionalServiceFee;
                    row[2] = item.DriverPercentageOnAddService;
                    row[3] = item.IsTaxAppliedOnAddionalService;
                    row[4] = item.TaxAmountOnAdditionalService;

                    additionalServiceList.Rows.Add(row);
                }
            }


            SqlParameter[] sqlParameters = {

                new SqlParameter("@OrderTypeId", SqlDbType.VarChar, 50) { Value = orderPoco.OrderTypeId },
                new SqlParameter("@ReferenceNumber", SqlDbType.VarChar, 50) { Value = (object)orderPoco.ReferenceNumber ?? DBNull.Value },
                new SqlParameter("@CargoCtlNumber", SqlDbType.Int) { Value = (object)orderPoco.CargoCtlNumber ?? DBNull.Value },
                new SqlParameter("@AwbCtnNumber", SqlDbType.VarChar, 50) { Value =(object) orderPoco.AwbCtnNumber ?? DBNull.Value },
                new SqlParameter("@ShipperCustomerId", SqlDbType.VarChar, 50) { Value = (object)orderPoco.ShipperCustomerId ?? DBNull.Value },
                new SqlParameter("@ConsigneeCustomerId", SqlDbType.VarChar, 50) { Value = (object)orderPoco.ConsigneeCustomerId ?? DBNull.Value },
                new SqlParameter("@BillToCustomerId", SqlDbType.VarChar, 50) { Value = (object)orderPoco.BillToCustomerId ?? DBNull.Value },
                new SqlParameter("@ScheduledPickupDate", SqlDbType.VarChar, 50) { Value =(object) orderPoco.ScheduledPickupDate ?? DBNull.Value },
                new SqlParameter("@ExpectedDeliveryDate", SqlDbType.VarChar, 150) { Value =(object) orderPoco.ExpectedDeliveryDate ?? DBNull.Value },
                new SqlParameter("@CityId", SqlDbType.Int) { Value = (object)orderPoco.CityId ?? DBNull.Value },
                new SqlParameter("@DeliveryOptionId", SqlDbType.Int) { Value = (object)orderPoco.DeliveryOptionId ?? DBNull.Value },
                new SqlParameter("@VehicleTypeId", SqlDbType.Int) { Value = (object)orderPoco.VehicleTypeId ?? DBNull.Value },
                new SqlParameter("@UnitTypeId", SqlDbType.VarChar, 50) { Value = (object)orderPoco.UnitTypeId ?? DBNull.Value },
                new SqlParameter("@WeightScaleId", SqlDbType.VarChar, 50) { Value = (object)orderPoco.WeightScaleId?? DBNull.Value  },
                new SqlParameter("@WeightTotal", SqlDbType.VarChar, 50) { Value = (object)orderPoco.WeightTotal?? DBNull.Value  },
                new SqlParameter("@UnitQuantity", SqlDbType.VarChar, 50) { Value = (object)orderPoco.UnitQuantity ?? DBNull.Value },
                new SqlParameter("@OrderBasicCost", SqlDbType.VarChar, 150) { Value = (object)orderPoco.OrderBasicCost ?? DBNull.Value },
                new SqlParameter("@BasicCostOverriden", SqlDbType.Int) { Value = (object)orderPoco.BasicCostOverriden?? DBNull.Value  },
                new SqlParameter("@FuelSurchargePercentage", SqlDbType.Bit) { Value = (object)orderPoco.FuelSurchargePercentage?? DBNull.Value  },
                new SqlParameter("@DiscountPercentOnOrderCost", SqlDbType.Decimal) { Value = (object)orderPoco.DiscountPercentOnOrderCost ?? DBNull.Value },
                new SqlParameter("@ApplicableGstPercent", SqlDbType.Bit, 50) { Value = (object)orderPoco.ApplicableGstPercent ?? DBNull.Value },
                new SqlParameter("@TotalOrderCost", SqlDbType.Decimal) { Value = (object)orderPoco.TotalOrderCost ?? DBNull.Value },
                new SqlParameter("@TotalAdditionalServiceCost", SqlDbType.Decimal) { Value = (object)orderPoco.TotalAdditionalServiceCost ?? DBNull.Value },
                new SqlParameter("@OrderedBy", SqlDbType.Decimal) { Value = (object)orderPoco.OrderedBy ?? DBNull.Value },
                new SqlParameter("@DepartmentName", SqlDbType.Int) { Value = (object)orderPoco.DepartmentName ?? DBNull.Value },
                new SqlParameter("@ContactName", SqlDbType.Bit) { Value =(object) orderPoco.ContactName ?? DBNull.Value },
                new SqlParameter("@ContactPhoneNumber", SqlDbType.Decimal) { Value = (object)orderPoco.ContactPhoneNumber ?? DBNull.Value },
                new SqlParameter("@Remarks", SqlDbType.Bit) { Value = (object)orderPoco.Remarks ?? DBNull.Value },
                new SqlParameter("@CreatedBy", SqlDbType.Decimal) { Value = (object)orderPoco.CreatedBy ?? DBNull.Value },
                new SqlParameter("@OrderAdditionalServiceList", SqlDbType.Structured) { TypeName = "dbo.", Value = additionalServiceList }

            };

            StringBuilder query = new StringBuilder();
            query.Append("EXEC CreateNewOrder ");
            query.Append("@OrderTypeId, @ReferenceNumber, @CargoCtlNumber, @AwbCtnNumber, @ShipperCustomerId, @ConsigneeCustomerId, ");
            query.Append("@BillToCustomerId, @ScheduledPickupDate, @ExpectedDeliveryDate, @CityId, @DeliveryOptionId, ");

            query.Append("@VehicleTypeId, @UnitTypeId, @WeightScaleId, @WeightTotal, @UnitQuantity, @OrderBasicCost, ");
            query.Append("@BasicCostOverriden, @FuelSurchargePercentage, @DiscountPercentOnOrderCost, @ApplicableGstPercent,@TotalOrderCost, ");

            query.Append("@TotalAdditionalServiceCost, @OrderedBy, @DepartmentName, @ContactName, @ContactPhoneNumber,  ");
            query.Append("@Remarks, @CreatedBy, @OrderAdditionalServiceList ");

            var outPut = base.CallStoredProcedure(query.ToString(), sqlParameters);

            _cache.Remove(App_CacheKeys.Employees);
            _cache.Remove(App_CacheKeys.Accounts);

            return outPut;
        }
    }
}
