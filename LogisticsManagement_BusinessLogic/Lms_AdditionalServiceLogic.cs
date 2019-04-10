using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AdditionalServiceLogic : BaseLogic<Lms_AdditionalServicePoco>
    {
        IMemoryCache _cache;
        public Lms_AdditionalServiceLogic(IMemoryCache cache, IDataRepository<Lms_AdditionalServicePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_AdditionalServicePoco> GetList()
        {
            List<Lms_AdditionalServicePoco> _additionalServices;
            if (!_cache.TryGetValue(App_CacheKeys.AdditionalServices, out _additionalServices))
            {
                _additionalServices = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.AdditionalServices, _additionalServices, cacheEntryOptions);
            }

            return _additionalServices;
        }

        public override List<Lms_AdditionalServicePoco> GetListById(int id)
        {
            List<Lms_AdditionalServicePoco> _additionalServices;
            if (!_cache.TryGetValue(App_CacheKeys.AdditionalServices, out _additionalServices))
            {
                _additionalServices = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.AdditionalServices, _additionalServices, cacheEntryOptions);
            }

            return _additionalServices;
        }

        public override Lms_AdditionalServicePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AdditionalServicePoco Add(Lms_AdditionalServicePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.AdditionalServices);

            return addedPoco;
        }

        public override Lms_AdditionalServicePoco Update(Lms_AdditionalServicePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.AdditionalServices);

            return updatedPoco;
        }

        public override void Remove(Lms_AdditionalServicePoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.AdditionalServices);
        }

        public override void Add(Lms_AdditionalServicePoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.AdditionalServices);
        }

        public override void Update(Lms_AdditionalServicePoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.AdditionalServices);
        }

        public override void Remove(Lms_AdditionalServicePoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.AdditionalServices);
        }

        #endregion


    }
}
