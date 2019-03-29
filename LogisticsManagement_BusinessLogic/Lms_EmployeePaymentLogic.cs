using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeePaymentLogic : BaseLogic<Lms_EmployeePaymentPoco>
    {
        public Lms_EmployeePaymentLogic(IDataRepository<Lms_EmployeePaymentPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeePaymentPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeePaymentPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeePaymentPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeePaymentPoco Add(Lms_EmployeePaymentPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_EmployeePaymentPoco Update(Lms_EmployeePaymentPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeePaymentPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeePaymentPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeePaymentPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeePaymentPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
