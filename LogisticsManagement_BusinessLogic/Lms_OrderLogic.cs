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
            _cache.Remove(App_CacheKeys.Orders);
            poco.CommentsForInvoice = !string.IsNullOrEmpty(poco.CommentsForInvoice) ? poco.CommentsForInvoice.ToUpper() : null;
            poco.ReferenceNumber = !string.IsNullOrEmpty(poco.ReferenceNumber) ? poco.ReferenceNumber.ToUpper() : null;
            poco.CargoCtlNumber = !string.IsNullOrEmpty(poco.CargoCtlNumber) ? poco.CargoCtlNumber.ToUpper() : null;
            poco.AwbCtnNumber = !string.IsNullOrEmpty(poco.AwbCtnNumber) ? poco.AwbCtnNumber.ToUpper() : null;
            poco.PickupReferenceNumber = !string.IsNullOrEmpty(poco.PickupReferenceNumber) ? poco.PickupReferenceNumber.ToUpper() : null;
            poco.OrderedBy = !string.IsNullOrEmpty(poco.OrderedBy) ? poco.OrderedBy.ToUpper() : null;
            poco.DepartmentName = !string.IsNullOrEmpty(poco.DepartmentName) ? poco.DepartmentName.ToUpper() : null;
            poco.ContactName = !string.IsNullOrEmpty(poco.ContactName) ? poco.ContactName.ToUpper() : null;
            poco.DeliveredBy = !string.IsNullOrEmpty(poco.DeliveredBy) ? poco.DeliveredBy.ToUpper() : null;
            poco.BolReferenceNumber = !string.IsNullOrEmpty(poco.BolReferenceNumber) ? poco.BolReferenceNumber.ToUpper() : null;
            poco.ProReferenceNumber = !string.IsNullOrEmpty(poco.ProReferenceNumber) ? poco.ProReferenceNumber.ToUpper() : null;
            poco.ShipperName = !string.IsNullOrEmpty(poco.ShipperName) ? poco.ShipperName.ToUpper() : null;
            poco.ShipperAddress = !string.IsNullOrEmpty(poco.ShipperAddress) ? poco.ShipperAddress.ToUpper() : null;
            poco.CommentsForWayBill = !string.IsNullOrEmpty(poco.CommentsForWayBill) ? poco.CommentsForWayBill.ToUpper() : null;
            poco.CommentsForInvoice = !string.IsNullOrEmpty(poco.CommentsForInvoice) ? poco.CommentsForInvoice.ToUpper() : null;
            return base.Add(poco);
        }

        public override Lms_OrderPoco Update(Lms_OrderPoco poco)
        {
            _cache.Remove(App_CacheKeys.Orders);
            poco.CommentsForInvoice = !string.IsNullOrEmpty(poco.CommentsForInvoice) ? poco.CommentsForInvoice.ToUpper() : null;
            poco.ReferenceNumber = !string.IsNullOrEmpty(poco.ReferenceNumber) ? poco.ReferenceNumber.ToUpper() : null;
            poco.CargoCtlNumber = !string.IsNullOrEmpty(poco.CargoCtlNumber) ? poco.CargoCtlNumber.ToUpper() : null;
            poco.AwbCtnNumber = !string.IsNullOrEmpty(poco.AwbCtnNumber) ? poco.AwbCtnNumber.ToUpper() : null;
            poco.PickupReferenceNumber = !string.IsNullOrEmpty(poco.PickupReferenceNumber) ? poco.PickupReferenceNumber.ToUpper() : null;
            poco.OrderedBy = !string.IsNullOrEmpty(poco.OrderedBy) ? poco.OrderedBy.ToUpper() : null;
            poco.DepartmentName = !string.IsNullOrEmpty(poco.DepartmentName) ? poco.DepartmentName.ToUpper() : null;
            poco.ContactName = !string.IsNullOrEmpty(poco.ContactName) ? poco.ContactName.ToUpper() : null;
            poco.DeliveredBy = !string.IsNullOrEmpty(poco.DeliveredBy) ? poco.DeliveredBy.ToUpper() : null;
            poco.BolReferenceNumber = !string.IsNullOrEmpty(poco.BolReferenceNumber) ? poco.BolReferenceNumber.ToUpper() : null;
            poco.ProReferenceNumber = !string.IsNullOrEmpty(poco.ProReferenceNumber) ? poco.ProReferenceNumber.ToUpper() : null;
            poco.ShipperName = !string.IsNullOrEmpty(poco.ShipperName) ? poco.ShipperName.ToUpper() : null;
            poco.ShipperAddress = !string.IsNullOrEmpty(poco.ShipperAddress) ? poco.ShipperAddress.ToUpper() : null;
            poco.CommentsForWayBill = !string.IsNullOrEmpty(poco.CommentsForWayBill) ? poco.CommentsForWayBill.ToUpper() : null;
            poco.CommentsForInvoice = !string.IsNullOrEmpty(poco.CommentsForInvoice) ? poco.CommentsForInvoice.ToUpper() : null;
            return base.Update(poco);
        }

        public override void Remove(Lms_OrderPoco poco)
        {
            _cache.Remove(App_CacheKeys.Orders);
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
    }
}
