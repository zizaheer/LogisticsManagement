using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CompanyInfoLogic : BaseLogic<Lms_CompanyInfoPoco>
    {
        IMemoryCache _cache;
        public Lms_CompanyInfoLogic(IMemoryCache cache, IDataRepository<Lms_CompanyInfoPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_CompanyInfoPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_CompanyInfoPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_CompanyInfoPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_CompanyInfoPoco Add(Lms_CompanyInfoPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_CompanyInfoPoco Update(Lms_CompanyInfoPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_CompanyInfoPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_CompanyInfoPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_CompanyInfoPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_CompanyInfoPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
