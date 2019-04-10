using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AccountBalanceLogic : BaseLogic<Lms_AccountBalancePoco>
    {
        IMemoryCache _cache;
        public Lms_AccountBalanceLogic(IMemoryCache cache, IDataRepository<Lms_AccountBalancePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_AccountBalancePoco> GetList()
        {
            List<Lms_AccountBalancePoco> _accountBalances;
            if (!_cache.TryGetValue(App_CacheKeys.AccountBalances, out _accountBalances))
            {
                _accountBalances = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.AccountBalances, _accountBalances, cacheEntryOptions);
            }

            return _accountBalances;
        }

        public override List<Lms_AccountBalancePoco> GetListById(int id)
        {
            List<Lms_AccountBalancePoco> _accountBalances;
            if (!_cache.TryGetValue(App_CacheKeys.AccountBalances, out _accountBalances))
            {
                _accountBalances = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.AccountBalances, _accountBalances, cacheEntryOptions);
            }

            return _accountBalances;
        }

        public override Lms_AccountBalancePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AccountBalancePoco Add(Lms_AccountBalancePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.AccountBalances);

            return addedPoco;
        }

        public override Lms_AccountBalancePoco Update(Lms_AccountBalancePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.AccountBalances);

            return updatedPoco;
        }

        public override void Remove(Lms_AccountBalancePoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.AccountBalances);
        }

        public override void Add(Lms_AccountBalancePoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.AccountBalances);
        }

        public override void Update(Lms_AccountBalancePoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.AccountBalances);
        }

        public override void Remove(Lms_AccountBalancePoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.AccountBalances);
        }

        #endregion


    }
}
