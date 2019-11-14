using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_TariffLogic : BaseLogic<Lms_TariffPoco>
    {
        IMemoryCache _cache;
        public Lms_TariffLogic(IMemoryCache cache, IDataRepository<Lms_TariffPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_TariffPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_TariffPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_TariffPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods


        public override Lms_TariffPoco Add(Lms_TariffPoco poco)
        {
            poco.CreateDate = DateTime.Now;
            _cache.Remove(App_CacheKeys.Tariffs);
            return base.Add(poco);
        }

        public override Lms_TariffPoco Update(Lms_TariffPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            _cache.Remove(App_CacheKeys.Tariffs);
            return base.Update(poco);
        }

        public override void Remove(Lms_TariffPoco poco)
        {
            _cache.Remove(App_CacheKeys.Tariffs);
            base.Remove(poco);
        }


        public override void Add(Lms_TariffPoco[] pocos)
        {
            _cache.Remove(App_CacheKeys.Tariffs);
            base.Add(pocos);
        }

        public override void Update(Lms_TariffPoco[] pocos)
        {
            _cache.Remove(App_CacheKeys.Tariffs);
            base.Update(pocos);
        }

        public override void Remove(Lms_TariffPoco[] pocos)
        {
            _cache.Remove(App_CacheKeys.Tariffs);
            base.Remove(pocos);
        }

        #endregion


    }
}
