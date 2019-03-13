using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CustomerLogic : BaseLogic<Lms_CustomerPoco>
    {
        public Lms_CustomerLogic(IDataRepository<Lms_CustomerPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_CustomerPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_CustomerPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_CustomerPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_CustomerPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_CustomerPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_CustomerPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
