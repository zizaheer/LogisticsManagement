﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AccountTypeLogic : BaseLogic<Lms_AccountTypePoco>
    {
        IMemoryCache _cache;
        public Lms_AccountTypeLogic(IMemoryCache cash, IDataRepository<Lms_AccountTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AccountTypePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_AccountTypePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_AccountTypePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AccountTypePoco Add(Lms_AccountTypePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_AccountTypePoco Update(Lms_AccountTypePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_AccountTypePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_AccountTypePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_AccountTypePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_AccountTypePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
