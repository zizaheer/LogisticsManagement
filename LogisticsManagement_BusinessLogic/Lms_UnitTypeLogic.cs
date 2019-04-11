using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_UnitTypeLogic : BaseLogic<Lms_UnitTypePoco>
    {
        IMemoryCache _cache;
        public Lms_UnitTypeLogic(IMemoryCache cash, IDataRepository<Lms_UnitTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_UnitTypePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_UnitTypePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_UnitTypePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_UnitTypePoco Add(Lms_UnitTypePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_UnitTypePoco Update(Lms_UnitTypePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_UnitTypePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_UnitTypePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_UnitTypePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_UnitTypePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
