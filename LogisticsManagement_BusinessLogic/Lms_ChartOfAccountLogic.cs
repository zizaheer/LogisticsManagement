using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_ChartOfAccountLogic : BaseLogic<Lms_ChartOfAccountPoco>
    {
        IMemoryCache _cache;
        public Lms_ChartOfAccountLogic(IMemoryCache cache, IDataRepository<Lms_ChartOfAccountPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_ChartOfAccountPoco> GetList()
        {
            List<Lms_ChartOfAccountPoco> _accounts;
            if (!_cache.TryGetValue(App_CacheKeys.Accounts, out _accounts))
            {
                _accounts = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Accounts, _accounts, cacheEntryOptions);
            }

            return _accounts;
        }

        public override List<Lms_ChartOfAccountPoco> GetListById(int id)
        {
            List<Lms_ChartOfAccountPoco> _accounts;
            if (!_cache.TryGetValue(App_CacheKeys.Accounts, out _accounts))
            {
                _accounts = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Accounts, _accounts, cacheEntryOptions);
            }

            return _accounts;
        }

        public override Lms_ChartOfAccountPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_ChartOfAccountPoco Add(Lms_ChartOfAccountPoco poco)
        {
            //var newAccount = GetList().Where(c => c.AccountTypeId == poco.AccountTypeId).OrderByDescending(c => c.Id).FirstOrDefault().AccountNo;
            //poco.AccountNo = (Convert.ToInt32(newAccount) + 1).ToString().PadLeft(8, '0');
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Accounts);

            return addedPoco;
        }

        public override Lms_ChartOfAccountPoco Update(Lms_ChartOfAccountPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Accounts);

            return updatedPoco;
        }

        public override void Remove(Lms_ChartOfAccountPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Accounts);
        }

        public override void Add(Lms_ChartOfAccountPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Accounts);
        }

        public override void Update(Lms_ChartOfAccountPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Accounts);
        }

        public override void Remove(Lms_ChartOfAccountPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Accounts);
        }

        #endregion


    }
}
