﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_BillPaymentLogic : BaseLogic<Lms_BillPaymentPoco>
    {
        IMemoryCache _cache;
        public Lms_BillPaymentLogic(IMemoryCache cash, IDataRepository<Lms_BillPaymentPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_BillPaymentPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_BillPaymentPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_BillPaymentPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_BillPaymentPoco Add(Lms_BillPaymentPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_BillPaymentPoco Update(Lms_BillPaymentPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_BillPaymentPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_BillPaymentPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_BillPaymentPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_BillPaymentPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
