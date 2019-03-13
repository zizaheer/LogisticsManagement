using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_UnitTypeLogic : BaseLogic<Lms_UnitTypePoco>
    {
        public Lms_UnitTypeLogic(IDataRepository<Lms_UnitTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_UnitTypePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_UnitTypePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_UnitTypePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_UnitTypePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_UnitTypePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_UnitTypePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
