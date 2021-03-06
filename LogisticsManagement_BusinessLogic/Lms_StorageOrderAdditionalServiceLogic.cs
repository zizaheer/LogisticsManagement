﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_StorageOrderAdditionalServiceLogic : BaseLogic<Lms_StorageOrderAdditionalServicePoco>
    {
        IMemoryCache _cache;
        public Lms_StorageOrderAdditionalServiceLogic(IMemoryCache cash, IDataRepository<Lms_StorageOrderAdditionalServicePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_StorageOrderAdditionalServicePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_StorageOrderAdditionalServicePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_StorageOrderAdditionalServicePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_StorageOrderAdditionalServicePoco Add(Lms_StorageOrderAdditionalServicePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_StorageOrderAdditionalServicePoco Update(Lms_StorageOrderAdditionalServicePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_StorageOrderAdditionalServicePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_StorageOrderAdditionalServicePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_StorageOrderAdditionalServicePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_StorageOrderAdditionalServicePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
