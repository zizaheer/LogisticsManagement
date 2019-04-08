using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AddressLogic : BaseLogic<Lms_AddressPoco>
    {
        IMemoryCache _cache;
        public Lms_AddressLogic(IMemoryCache cache, IDataRepository<Lms_AddressPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_AddressPoco> GetList()
        {
            List<Lms_AddressPoco> _addresses;
            if (!_cache.TryGetValue(App_CacheKeys.Addresses, out _addresses))
            {
                _addresses = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Addresses, _addresses, cacheEntryOptions);
            }

            return _addresses;
        }

        public override List<Lms_AddressPoco> GetListById(int id)
        {
            List<Lms_AddressPoco> _addresses;
            if (!_cache.TryGetValue(App_CacheKeys.Addresses, out _addresses))
            {
                _addresses = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Addresses, _addresses, cacheEntryOptions);
            }

            return _addresses;
        }

        public override Lms_AddressPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AddressPoco Add(Lms_AddressPoco poco)
        {
            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Addresses);

            return addedPoco;
        }

        public override Lms_AddressPoco Update(Lms_AddressPoco poco)
        {
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Addresses);

            return updatedPoco;
        }

        public override void Remove(Lms_AddressPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Addresses);
        }

        public override void Add(Lms_AddressPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Addresses);
        }

        public override void Update(Lms_AddressPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Addresses);
        }

        public override void Remove(Lms_AddressPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Addresses);
        }

        #endregion


    }
}
