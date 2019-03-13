using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeLogic : BaseLogic<Lms_EmployeePoco>
    {
        public Lms_EmployeeLogic(IDataRepository<Lms_EmployeePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_EmployeePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_EmployeePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_EmployeePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_EmployeePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_EmployeePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
