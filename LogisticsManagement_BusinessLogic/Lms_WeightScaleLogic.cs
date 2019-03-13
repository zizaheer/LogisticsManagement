using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_WeightScaleLogic : BaseLogic<Lms_WeightScalePoco>
    {
        public Lms_WeightScaleLogic(IDataRepository<Lms_WeightScalePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_WeightScalePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_WeightScalePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_WeightScalePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_WeightScalePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_WeightScalePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_WeightScalePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion



    }
}
