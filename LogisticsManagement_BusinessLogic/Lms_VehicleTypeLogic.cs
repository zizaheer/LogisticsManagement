﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_VehicleTypeLogic : BaseLogic<Lms_VehicleTypePoco>
    {
        public Lms_VehicleTypeLogic(IDataRepository<Lms_VehicleTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_VehicleTypePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_VehicleTypePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_VehicleTypePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_VehicleTypePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_VehicleTypePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_VehicleTypePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
