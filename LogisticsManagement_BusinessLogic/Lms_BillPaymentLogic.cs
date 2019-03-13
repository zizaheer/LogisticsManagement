﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_BillPaymentLogic : BaseLogic<Lms_BillPaymentPoco>
    {
        public Lms_BillPaymentLogic(IDataRepository<Lms_BillPaymentPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_BillPaymentPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_BillPaymentPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_BillPaymentPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_BillPaymentPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_BillPaymentPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_BillPaymentPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
