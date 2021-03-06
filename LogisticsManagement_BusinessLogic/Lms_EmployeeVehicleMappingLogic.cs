﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeVehicleMappingLogic : BaseLogic<Lms_EmployeeVehicleMappingPoco>
    {
        IMemoryCache _cache;
        public Lms_EmployeeVehicleMappingLogic(IMemoryCache cash, IDataRepository<Lms_EmployeeVehicleMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeeVehicleMappingPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeeVehicleMappingPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeeVehicleMappingPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeeVehicleMappingPoco Add(Lms_EmployeeVehicleMappingPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_EmployeeVehicleMappingPoco Update(Lms_EmployeeVehicleMappingPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeeVehicleMappingPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeeVehicleMappingPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeeVehicleMappingPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeeVehicleMappingPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
