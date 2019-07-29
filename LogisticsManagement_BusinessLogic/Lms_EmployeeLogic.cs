using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;
using System.Data;

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
            List<Lms_EmployeePoco> _employees;
            if (!_cache.TryGetValue(App_CacheKeys.Employees, out _employees))
            {
                _employees = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Employees, _employees, cacheEntryOptions);
            }

            return _employees;
        }

        public override List<Lms_EmployeePoco> GetListById(int id)
        {
            List<Lms_EmployeePoco> _employees;
            if (!_cache.TryGetValue(App_CacheKeys.Employees, out _employees))
            {
                _employees = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Employees, _employees, cacheEntryOptions);
            }

            return _employees;
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
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Employees);
            _cache.Remove(App_CacheKeys.Accounts);

            return addedPoco;
        }

        public override Lms_EmployeePoco Update(Lms_EmployeePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Employees);
            _cache.Remove(App_CacheKeys.Accounts);

            return updatedPoco;
        }

        public override void Remove(Lms_EmployeePoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Employees);
            _cache.Remove(App_CacheKeys.Accounts);
        }

        public override void Add(Lms_EmployeePoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Employees);
            _cache.Remove(App_CacheKeys.Accounts);
        }

        public override void Update(Lms_EmployeePoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Employees);
            _cache.Remove(App_CacheKeys.Accounts);
        }

        public override void Remove(Lms_EmployeePoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Employees);
            _cache.Remove(App_CacheKeys.Accounts);
        }

        #endregion

    }
}
