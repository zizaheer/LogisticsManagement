using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_VehicleUnitMappingLogic : BaseLogic<Lms_VehicleUnitMappingPoco>
    {
        IMemoryCache _cache;
        public Lms_VehicleUnitMappingLogic(IMemoryCache cash, IDataRepository<Lms_VehicleUnitMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_VehicleUnitMappingPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_VehicleUnitMappingPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_VehicleUnitMappingPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_VehicleUnitMappingPoco Add(Lms_VehicleUnitMappingPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_VehicleUnitMappingPoco Update(Lms_VehicleUnitMappingPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_VehicleUnitMappingPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_VehicleUnitMappingPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_VehicleUnitMappingPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_VehicleUnitMappingPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion
    }
}
