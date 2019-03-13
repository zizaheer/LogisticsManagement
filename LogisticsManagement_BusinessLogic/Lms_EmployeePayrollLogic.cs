using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeePayrollLogic : BaseLogic<Lms_EmployeePayrollPoco>
    {
        public Lms_EmployeePayrollLogic(IDataRepository<Lms_EmployeePayrollPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeePayrollPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_EmployeePayrollPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_EmployeePayrollPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_EmployeePayrollPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_EmployeePayrollPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_EmployeePayrollPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
