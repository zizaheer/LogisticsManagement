﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_ChartOfAccountLogic : BaseLogic<Lms_ChartOfAccountPoco>
    {
        public Lms_ChartOfAccountLogic(IDataRepository<Lms_ChartOfAccountPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_ChartOfAccountPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_ChartOfAccountPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_ChartOfAccountPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_ChartOfAccountPoco Add(Lms_ChartOfAccountPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_ChartOfAccountPoco Update(Lms_ChartOfAccountPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_ChartOfAccountPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_ChartOfAccountPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_ChartOfAccountPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_ChartOfAccountPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion

    }
}
