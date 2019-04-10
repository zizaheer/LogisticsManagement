using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class App_ProvinceLogic : BaseLogic<App_ProvincePoco>
    {
        IMemoryCache _cache;
        public App_ProvinceLogic(IMemoryCache cache, IDataRepository<App_ProvincePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<App_ProvincePoco> GetList()
        {
            List<App_ProvincePoco> _provinces;
            if (!_cache.TryGetValue(App_CacheKeys.Provinces, out _provinces))
            {
                _provinces = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Provinces, _provinces, cacheEntryOptions);
            }

            return _provinces;
        }

        public override List<App_ProvincePoco> GetListById(int id)
        {
            List<App_ProvincePoco> _provinces;
            if (!_cache.TryGetValue(App_CacheKeys.Provinces, out _provinces))
            {
                _provinces = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Provinces, _provinces, cacheEntryOptions);
            }

            return _provinces;
        }

        public override App_ProvincePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override App_ProvincePoco Add(App_ProvincePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Provinces);

            return addedPoco;
        }

        public override App_ProvincePoco Update(App_ProvincePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Provinces);

            return updatedPoco;
        }

        public override void Remove(App_ProvincePoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Provinces);
        }

        public override void Add(App_ProvincePoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Provinces);
        }

        public override void Update(App_ProvincePoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Provinces);
        }

        public override void Remove(App_ProvincePoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Provinces);
        }

        #endregion


    }
}
