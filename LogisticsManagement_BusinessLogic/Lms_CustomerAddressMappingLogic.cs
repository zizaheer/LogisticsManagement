using System;
using System.Collections.Generic;
using System.Text;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CustomerAddressMappingLogic : BaseLogic<Lms_CustomerAddressMappingPoco>
    {
        IMemoryCache _cache;
        public Lms_CustomerAddressMappingLogic(IMemoryCache cache, IDataRepository<Lms_CustomerAddressMappingPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_CustomerAddressMappingPoco> GetList()
        {
            List<Lms_CustomerAddressMappingPoco> _addresses;
            if (!_cache.TryGetValue(App_CacheKeys.CustomerAddresses, out _addresses))
            {
                _addresses = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.CustomerAddresses, _addresses, cacheEntryOptions);
            }

            return _addresses;
        }

        public override List<Lms_CustomerAddressMappingPoco> GetListById(int id)
        {
            List<Lms_CustomerAddressMappingPoco> _addresses;
            if (!_cache.TryGetValue(App_CacheKeys.CustomerAddresses, out _addresses))
            {
                _addresses = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.CustomerAddresses, _addresses, cacheEntryOptions);
            }

            return _addresses;
        }

        public override Lms_CustomerAddressMappingPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_CustomerAddressMappingPoco Add(Lms_CustomerAddressMappingPoco poco)
        {
            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.CustomerAddresses);

            return addedPoco;
        }

        public override Lms_CustomerAddressMappingPoco Update(Lms_CustomerAddressMappingPoco poco)
        {
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.CustomerAddresses);

            return updatedPoco;
        }

        public override void Remove(Lms_CustomerAddressMappingPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.CustomerAddresses);
        }

        public override void Add(Lms_CustomerAddressMappingPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.CustomerAddresses);
        }

        public override void Update(Lms_CustomerAddressMappingPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.CustomerAddresses);
        }

        public override void Remove(Lms_CustomerAddressMappingPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.CustomerAddresses);
        }

        #endregion
    }
}
