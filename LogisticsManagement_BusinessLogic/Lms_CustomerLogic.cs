using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CustomerLogic : BaseLogic<Lms_CustomerPoco>
    {
        IMemoryCache _cache;
        public Lms_CustomerLogic(IMemoryCache cache, IDataRepository<Lms_CustomerPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_CustomerPoco> GetList()
        {
            List<Lms_CustomerPoco> _customers;
            if(!_cache.TryGetValue(App_CacheKeys.Customers, out _customers))
            {
                _customers = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            return _customers;
        }

        public override List<Lms_CustomerPoco> GetListById(int id)
        {
            List<Lms_CustomerPoco> _customers;
            if (!_cache.TryGetValue(App_CacheKeys.Customers, out _customers))
            {
                _customers = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            return _customers;
        }

        public override Lms_CustomerPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_CustomerPoco Add(Lms_CustomerPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Customers);

            return addedPoco;
        }

        public override Lms_CustomerPoco Update(Lms_CustomerPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Customers);

            return updatedPoco;
        }

        public override void Remove(Lms_CustomerPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Add(Lms_CustomerPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Update(Lms_CustomerPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Remove(Lms_CustomerPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        #endregion

        public  string GetCustomerData(Lms_CustomerPoco parameters)
        {
            string jsonOutPut = "";
            var param1 = new SqlParameter("@Query", parameters.CustomerName);
            var outPut = base.CallStoredProcedure("EXEC SearchCustomerByName @Query", param1 );

            return outPut;
        }
    }
}
