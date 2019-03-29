using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeTimesheetLogic : BaseLogic<Lms_EmployeeTimesheetPoco>
    {
        public Lms_EmployeeTimesheetLogic(IDataRepository<Lms_EmployeeTimesheetPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeeTimesheetPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeeTimesheetPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeeTimesheetPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeeTimesheetPoco Add(Lms_EmployeeTimesheetPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_EmployeeTimesheetPoco Update(Lms_EmployeeTimesheetPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeeTimesheetPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeeTimesheetPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeeTimesheetPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeeTimesheetPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
