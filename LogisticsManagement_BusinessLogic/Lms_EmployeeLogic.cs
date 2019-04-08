using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeLogic : BaseLogic<Lms_EmployeePoco>
    {
        IMemoryCache _cache;
        public Lms_EmployeeLogic(IMemoryCache cache, IDataRepository<Lms_EmployeePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_EmployeePoco> GetList()
        {
            List<Lms_EmployeePoco> _customers;
            if (!_cache.TryGetValue(App_CacheKeys.Customers, out _customers))
            {
                _customers = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            return _customers;
        }

        public override List<Lms_EmployeePoco> GetListById(int id)
        {
            List<Lms_EmployeePoco> _customers;
            if (!_cache.TryGetValue(App_CacheKeys.Customers, out _customers))
            {
                _customers = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            return _customers;
        }

        public override Lms_EmployeePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeePoco Add(Lms_EmployeePoco poco)
        {
            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Customers);

            return addedPoco;
        }

        public override Lms_EmployeePoco Update(Lms_EmployeePoco poco)
        {
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Customers);

            return updatedPoco;
        }

        public override void Remove(Lms_EmployeePoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Add(Lms_EmployeePoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Update(Lms_EmployeePoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Remove(Lms_EmployeePoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        #endregion


    }
}
