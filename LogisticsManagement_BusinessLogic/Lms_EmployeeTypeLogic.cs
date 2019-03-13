using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeTypeLogic : BaseLogic<Lms_EmployeeTypePoco>
    {
        public Lms_EmployeeTypeLogic(IDataRepository<Lms_EmployeeTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeeTypePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_EmployeeTypePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_EmployeeTypePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_EmployeeTypePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_EmployeeTypePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_EmployeeTypePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
