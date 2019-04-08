using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeePayrollDetailLogic : BaseLogic<Lms_EmployeePayrollDetailPoco>
    {
        IMemoryCache _cache;
        public Lms_EmployeePayrollDetailLogic(IMemoryCache cache, IDataRepository<Lms_EmployeePayrollDetailPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_EmployeePayrollDetailPoco> GetList()
        {
            List<Lms_EmployeePayrollDetailPoco> _employeePayrollDetails;
            if (!_cache.TryGetValue(App_CacheKeys.EmployeePayrollDetails, out _employeePayrollDetails))
            {
                _employeePayrollDetails = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.EmployeePayrollDetails, _employeePayrollDetails, cacheEntryOptions);
            }

            return _employeePayrollDetails;
        }

        public override List<Lms_EmployeePayrollDetailPoco> GetListById(int id)
        {
            List<Lms_EmployeePayrollDetailPoco> _employeePayrollDetails;
            if (!_cache.TryGetValue(App_CacheKeys.EmployeePayrollDetails, out _employeePayrollDetails))
            {
                _employeePayrollDetails = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.EmployeePayrollDetails, _employeePayrollDetails, cacheEntryOptions);
            }

            return _employeePayrollDetails;
        }

        public override Lms_EmployeePayrollDetailPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeePayrollDetailPoco Add(Lms_EmployeePayrollDetailPoco poco)
        {
            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.EmployeePayrollDetails);

            return addedPoco;
        }

        public override Lms_EmployeePayrollDetailPoco Update(Lms_EmployeePayrollDetailPoco poco)
        {
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.EmployeePayrollDetails);

            return updatedPoco;
        }

        public override void Remove(Lms_EmployeePayrollDetailPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.EmployeePayrollDetails);
        }

        public override void Add(Lms_EmployeePayrollDetailPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.EmployeePayrollDetails);
        }

        public override void Update(Lms_EmployeePayrollDetailPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.EmployeePayrollDetails);
        }

        public override void Remove(Lms_EmployeePayrollDetailPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.EmployeePayrollDetails);
        }

        #endregion


    }
}
