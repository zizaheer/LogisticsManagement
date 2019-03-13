using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeePayrollDetailLogic : BaseLogic<Lms_EmployeePayrollDetailPoco>
    {
        public Lms_EmployeePayrollDetailLogic(IDataRepository<Lms_EmployeePayrollDetailPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeePayrollDetailPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_EmployeePayrollDetailPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_EmployeePayrollDetailPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_EmployeePayrollDetailPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_EmployeePayrollDetailPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_EmployeePayrollDetailPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
