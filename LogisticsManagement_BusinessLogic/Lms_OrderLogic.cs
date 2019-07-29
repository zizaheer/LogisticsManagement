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
            return base.Add(poco);
        }

        public override Lms_OrderPoco Update(Lms_OrderPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            _cache.Remove(App_CacheKeys.Orders);
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
