﻿using LogisticsManagement_Poco;
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

        public override List<Lms_EmployeeTimesheetPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_EmployeeTimesheetPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_EmployeeTimesheetPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_EmployeeTimesheetPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_EmployeeTimesheetPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_EmployeeTimesheetPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
