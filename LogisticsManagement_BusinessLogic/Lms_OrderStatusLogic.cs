using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_OrderStatusLogic : BaseLogic<Lms_OrderStatusPoco>
    {
        public Lms_OrderStatusLogic(IDataRepository<Lms_OrderStatusPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_OrderStatusPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_OrderStatusPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_OrderStatusPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_OrderStatusPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_OrderStatusPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_OrderStatusPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
