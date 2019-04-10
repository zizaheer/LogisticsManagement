using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_InvoiceLogic : BaseLogic<Lms_InvoicePoco>
    {
        IMemoryCache _cache;
        public Lms_InvoiceLogic(IMemoryCache cache, IDataRepository<Lms_InvoicePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_InvoicePoco> GetList()
        {
            List<Lms_InvoicePoco> _invoices;
            if (!_cache.TryGetValue(App_CacheKeys.Invoices, out _invoices))
            {
                _invoices = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Invoices, _invoices, cacheEntryOptions);
            }

            return _invoices;
        }

        public override List<Lms_InvoicePoco> GetListById(int id)
        {
            List<Lms_InvoicePoco> _invoices;
            if (!_cache.TryGetValue(App_CacheKeys.Invoices, out _invoices))
            {
                _invoices = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Invoices, _invoices, cacheEntryOptions);
            }

            return _invoices;
        }

        public override Lms_InvoicePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_InvoicePoco Add(Lms_InvoicePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Invoices);

            return addedPoco;
        }

        public override Lms_InvoicePoco Update(Lms_InvoicePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Invoices);

            return updatedPoco;
        }

        public override void Remove(Lms_InvoicePoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Add(Lms_InvoicePoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Update(Lms_InvoicePoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Remove(Lms_InvoicePoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        #endregion


    }
}
