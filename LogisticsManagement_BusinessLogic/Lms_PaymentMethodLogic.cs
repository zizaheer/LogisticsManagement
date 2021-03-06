﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_PaymentMethodLogic : BaseLogic<Lms_PaymentMethodPoco>
    {
        IMemoryCache _cache;
        public Lms_PaymentMethodLogic(IMemoryCache cash, IDataRepository<Lms_PaymentMethodPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_PaymentMethodPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_PaymentMethodPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_PaymentMethodPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_PaymentMethodPoco Add(Lms_PaymentMethodPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_PaymentMethodPoco Update(Lms_PaymentMethodPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_PaymentMethodPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_PaymentMethodPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_PaymentMethodPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_PaymentMethodPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion
    }
}
