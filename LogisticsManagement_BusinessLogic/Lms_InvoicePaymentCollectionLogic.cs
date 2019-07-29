using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Data.SqlClient;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_InvoicePaymentCollectionLogic : BaseLogic<Lms_InvoicePaymentCollectionPoco>
    {
        IMemoryCache _cache;
        public Lms_InvoicePaymentCollectionLogic(IMemoryCache cache, IDataRepository<Lms_InvoicePaymentCollectionPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_InvoicePaymentCollectionPoco> GetList()
        {
            List<Lms_InvoicePaymentCollectionPoco> _paymentCollections;
            if (!_cache.TryGetValue(App_CacheKeys.Invoices, out _paymentCollections))
            {
                _paymentCollections = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Invoices, _paymentCollections, cacheEntryOptions);
            }

            return _paymentCollections;
        }

        public override List<Lms_InvoicePaymentCollectionPoco> GetListById(int id)
        {
            List<Lms_InvoicePaymentCollectionPoco> _paymentCollections;
            if (!_cache.TryGetValue(App_CacheKeys.Invoices, out _paymentCollections))
            {
                _paymentCollections = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Invoices, _paymentCollections, cacheEntryOptions);
            }

            return _paymentCollections;
        }

        public override Lms_InvoicePaymentCollectionPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_InvoicePaymentCollectionPoco Add(Lms_InvoicePaymentCollectionPoco poco)
        {
            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Invoices);

            return addedPoco;
        }

        public override Lms_InvoicePaymentCollectionPoco Update(Lms_InvoicePaymentCollectionPoco poco)
        {
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Invoices);

            return updatedPoco;
        }

        public override void Remove(Lms_InvoicePaymentCollectionPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Add(Lms_InvoicePaymentCollectionPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Update(Lms_InvoicePaymentCollectionPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Remove(Lms_InvoicePaymentCollectionPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        #endregion

    }
}


