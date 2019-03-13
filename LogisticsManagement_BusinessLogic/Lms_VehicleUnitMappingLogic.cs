using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_VehicleUnitMappingLogic : BaseLogic<Lms_VehicleUnitMappingPoco>
    {
        public Lms_VehicleUnitMappingLogic(IDataRepository<Lms_VehicleUnitMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_VehicleUnitMappingPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_VehicleUnitMappingPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_VehicleUnitMappingPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_VehicleUnitMappingPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_VehicleUnitMappingPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_VehicleUnitMappingPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
