using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AdditionalServiceLogic : BaseLogic<Lms_AdditionalServicePoco>
    {
        public Lms_AdditionalServiceLogic(IDataRepository<Lms_AdditionalServicePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AdditionalServicePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_AdditionalServicePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_AdditionalServicePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_AdditionalServicePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_AdditionalServicePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_AdditionalServicePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
