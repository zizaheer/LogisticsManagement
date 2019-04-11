using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_VehicleTypeLogic : BaseLogic<Lms_VehicleTypePoco>
    {
        IMemoryCache _cache;
        public Lms_VehicleTypeLogic(IMemoryCache cash, IDataRepository<Lms_VehicleTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_VehicleTypePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_VehicleTypePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_VehicleTypePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_VehicleTypePoco Add(Lms_VehicleTypePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_VehicleTypePoco Update(Lms_VehicleTypePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_VehicleTypePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_VehicleTypePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_VehicleTypePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_VehicleTypePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
