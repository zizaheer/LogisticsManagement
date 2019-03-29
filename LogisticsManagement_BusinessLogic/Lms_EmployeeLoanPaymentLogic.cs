﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeLoanPaymentLogic : BaseLogic<Lms_EmployeeLoanPaymentPoco>
    {
        public Lms_EmployeeLoanPaymentLogic(IDataRepository<Lms_EmployeeLoanPaymentPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeeLoanPaymentPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeeLoanPaymentPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeeLoanPaymentPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeeLoanPaymentPoco Add(Lms_EmployeeLoanPaymentPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_EmployeeLoanPaymentPoco Update(Lms_EmployeeLoanPaymentPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeeLoanPaymentPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeeLoanPaymentPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeeLoanPaymentPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeeLoanPaymentPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
