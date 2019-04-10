using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class App_CityLogic : BaseLogic<App_CityPoco>
    {
        IMemoryCache _cache;
        public App_CityLogic(IMemoryCache cache, IDataRepository<App_CityPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<App_CityPoco> GetList()
        {
            List<App_CityPoco> _cities;
            if (!_cache.TryGetValue(App_CacheKeys.Cities, out _cities))
            {
                _cities = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Cities, _cities, cacheEntryOptions);
            }

            return _cities;
        }

        public override List<App_CityPoco> GetListById(int id)
        {
            List<App_CityPoco> _cities;
            if (!_cache.TryGetValue(App_CacheKeys.Cities, out _cities))
            {
                _cities = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Cities, _cities, cacheEntryOptions);
            }

            return _cities;
        }

        public override App_CityPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override App_CityPoco Add(App_CityPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Cities);

            return addedPoco;
        }

        public override App_CityPoco Update(App_CityPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Cities);

            return updatedPoco;
        }

        public override void Remove(App_CityPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Cities);
        }

        public override void Add(App_CityPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Cities);
        }

        public override void Update(App_CityPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Cities);
        }

        public override void Remove(App_CityPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Cities);
        }

        #endregion


    }
}
