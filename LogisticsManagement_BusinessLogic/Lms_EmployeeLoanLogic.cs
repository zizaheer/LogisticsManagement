using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeLoanLogic : BaseLogic<Lms_EmployeeLoanPoco>
    {
        public Lms_EmployeeLoanLogic(IDataRepository<Lms_EmployeeLoanPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeeLoanPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_EmployeeLoanPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_EmployeeLoanPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_EmployeeLoanPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_EmployeeLoanPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_EmployeeLoanPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
