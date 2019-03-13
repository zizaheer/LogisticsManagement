using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_OrderTypeLogic : BaseLogic<Lms_OrderTypePoco>
    {
        public Lms_OrderTypeLogic(IDataRepository<Lms_OrderTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_OrderTypePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_OrderTypePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_OrderTypePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_OrderTypePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_OrderTypePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_OrderTypePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
