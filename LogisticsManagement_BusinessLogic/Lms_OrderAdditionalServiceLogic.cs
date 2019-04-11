using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_OrderAdditionalServiceLogic : BaseLogic<Lms_OrderAdditionalServicePoco>
    {
        IMemoryCache _cache;
        public Lms_OrderAdditionalServiceLogic(IMemoryCache cache, IDataRepository<Lms_OrderAdditionalServicePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_OrderAdditionalServicePoco> GetList()
        {
            List<Lms_OrderAdditionalServicePoco> _orderAdditionalServices;
            if (!_cache.TryGetValue(App_CacheKeys.OrderAdditionalServices, out _orderAdditionalServices))
            {
                _orderAdditionalServices = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.OrderAdditionalServices, _orderAdditionalServices, cacheEntryOptions);
            }

            return _orderAdditionalServices;
        }

        public override List<Lms_OrderAdditionalServicePoco> GetListById(int id)
        {
            List<Lms_OrderAdditionalServicePoco> _orderAdditionalServices;
            if (!_cache.TryGetValue(App_CacheKeys.OrderAdditionalServices, out _orderAdditionalServices))
            {
                _orderAdditionalServices = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.OrderAdditionalServices, _orderAdditionalServices, cacheEntryOptions);
            }

            return _orderAdditionalServices;
        }

        public override Lms_OrderAdditionalServicePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_OrderAdditionalServicePoco Add(Lms_OrderAdditionalServicePoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_OrderAdditionalServicePoco Update(Lms_OrderAdditionalServicePoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_OrderAdditionalServicePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_OrderAdditionalServicePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_OrderAdditionalServicePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_OrderAdditionalServicePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
