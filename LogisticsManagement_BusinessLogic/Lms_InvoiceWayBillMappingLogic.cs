using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_InvoiceWayBillMappingLogic : BaseLogic<Lms_InvoiceWayBillMappingPoco>
    {
        IMemoryCache _cache;
        public Lms_InvoiceWayBillMappingLogic(IMemoryCache cache, IDataRepository<Lms_InvoiceWayBillMappingPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_InvoiceWayBillMappingPoco> GetList()
        {
            List<Lms_InvoiceWayBillMappingPoco> _invoiceMappings;
            if (!_cache.TryGetValue(App_CacheKeys.InvoiceWayBillMappings, out _invoiceMappings))
            {
                _invoiceMappings = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.InvoiceWayBillMappings, _invoiceMappings, cacheEntryOptions);
            }

            return _invoiceMappings;
        }

        public override List<Lms_InvoiceWayBillMappingPoco> GetListById(int id)
        {
            List<Lms_InvoiceWayBillMappingPoco> _invoiceMappings;
            if (!_cache.TryGetValue(App_CacheKeys.InvoiceWayBillMappings, out _invoiceMappings))
            {
                _invoiceMappings = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.InvoiceWayBillMappings, _invoiceMappings, cacheEntryOptions);
            }

            return _invoiceMappings;
        }

        public override Lms_InvoiceWayBillMappingPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_InvoiceWayBillMappingPoco Add(Lms_InvoiceWayBillMappingPoco poco)
        {
            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.InvoiceWayBillMappings);

            return addedPoco;
        }

        public override Lms_InvoiceWayBillMappingPoco Update(Lms_InvoiceWayBillMappingPoco poco)
        {

            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.InvoiceWayBillMappings);

            return updatedPoco;
        }

        public override void Remove(Lms_InvoiceWayBillMappingPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.InvoiceWayBillMappings);
        }

        public override void Add(Lms_InvoiceWayBillMappingPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.InvoiceWayBillMappings);
        }

        public override void Update(Lms_InvoiceWayBillMappingPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.InvoiceWayBillMappings);
        }

        public override void Remove(Lms_InvoiceWayBillMappingPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.InvoiceWayBillMappings);
        }

        #endregion


    }
}
