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

        public override List<Lms_EmployeePaymentPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_EmployeePaymentPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_EmployeePaymentPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_EmployeePaymentPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_EmployeePaymentPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_EmployeePaymentPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
