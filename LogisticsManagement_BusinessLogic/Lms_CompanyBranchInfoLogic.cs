﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CompanyBranchInfoLogic : BaseLogic<Lms_CompanyBranchInfoPoco>
    {
        IMemoryCache _cache;
        public Lms_CompanyBranchInfoLogic(IMemoryCache cash, IDataRepository<Lms_CompanyBranchInfoPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_CompanyBranchInfoPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_CompanyBranchInfoPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_CompanyBranchInfoPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_CompanyBranchInfoPoco Add(Lms_CompanyBranchInfoPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_CompanyBranchInfoPoco Update(Lms_CompanyBranchInfoPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_CompanyBranchInfoPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_CompanyBranchInfoPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_CompanyBranchInfoPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_CompanyBranchInfoPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
