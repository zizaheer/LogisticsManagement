using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class App_CountryLogic : BaseLogic<App_CountryPoco>
    {
        IMemoryCache _cache;
        public App_CountryLogic(IMemoryCache cache, IDataRepository<App_CountryPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<App_CountryPoco> GetList()
        {
            List<App_CountryPoco> _countries;
            if (!_cache.TryGetValue(App_CacheKeys.Countries, out _countries))
            {
                _countries = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Countries, _countries, cacheEntryOptions);
            }

            return _countries;
        }

        public override List<App_CountryPoco> GetListById(int id)
        {
            List<App_CountryPoco> _countries;
            if (!_cache.TryGetValue(App_CacheKeys.Countries, out _countries))
            {
                _countries = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Countries, _countries, cacheEntryOptions);
            }

            return _countries;
        }

        public override App_CountryPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override App_CountryPoco Add(App_CountryPoco poco)
        {
            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Countries);

            return addedPoco;
        }

        public override App_CountryPoco Update(App_CountryPoco poco)
        {
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Countries);

            return updatedPoco;
        }

        public override void Remove(App_CountryPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Countries);
        }

        public override void Add(App_CountryPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Countries);
        }

        public override void Update(App_CountryPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Countries);
        }

        public override void Remove(App_CountryPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Countries);
        }

        #endregion


    }
}
