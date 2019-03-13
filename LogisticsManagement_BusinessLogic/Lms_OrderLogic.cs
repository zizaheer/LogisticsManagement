using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_OrderLogic : BaseLogic<Lms_OrderPoco>
    {
        public Lms_OrderLogic(IDataRepository<Lms_OrderPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_OrderPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_OrderPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_OrderPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_OrderPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_OrderPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_OrderPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
