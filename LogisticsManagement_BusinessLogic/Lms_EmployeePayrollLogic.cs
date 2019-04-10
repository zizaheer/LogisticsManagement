using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeePayrollLogic : BaseLogic<Lms_EmployeePayrollPoco>
    {
        IMemoryCache _cache;
        public Lms_EmployeePayrollLogic(IMemoryCache cache, IDataRepository<Lms_EmployeePayrollPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_EmployeePayrollPoco> GetList()
        {
            List<Lms_EmployeePayrollPoco> _employeePayrolls;
            if (!_cache.TryGetValue(App_CacheKeys.EmployeePayrolls, out _employeePayrolls))
            {
                _employeePayrolls = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.EmployeePayrolls, _employeePayrolls, cacheEntryOptions);
            }

            return _employeePayrolls;
        }

        public override List<Lms_EmployeePayrollPoco> GetListById(int id)
        {
            List<Lms_EmployeePayrollPoco> _employeePayrolls;
            if (!_cache.TryGetValue(App_CacheKeys.EmployeePayrolls, out _employeePayrolls))
            {
                _employeePayrolls = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.EmployeePayrolls, _employeePayrolls, cacheEntryOptions);
            }

            return _employeePayrolls;
        }

        public override Lms_EmployeePayrollPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeePayrollPoco Add(Lms_EmployeePayrollPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.EmployeePayrolls);

            return addedPoco;
        }

        public override Lms_EmployeePayrollPoco Update(Lms_EmployeePayrollPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.EmployeePayrolls);

            return updatedPoco;
        }

        public override void Remove(Lms_EmployeePayrollPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.EmployeePayrolls);
        }

        public override void Add(Lms_EmployeePayrollPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.EmployeePayrolls);
        }

        public override void Update(Lms_EmployeePayrollPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.EmployeePayrolls);
        }

        public override void Remove(Lms_EmployeePayrollPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.EmployeePayrolls);
        }

        #endregion


    }
}
