using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeVehicleMappingLogic : BaseLogic<Lms_EmployeeVehicleMappingPoco>
    {
        public Lms_EmployeeVehicleMappingLogic(IDataRepository<Lms_EmployeeVehicleMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeeVehicleMappingPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_EmployeeVehicleMappingPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_EmployeeVehicleMappingPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_EmployeeVehicleMappingPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_EmployeeVehicleMappingPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_EmployeeVehicleMappingPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
