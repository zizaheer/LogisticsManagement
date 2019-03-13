using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AccountTypeLogic : BaseLogic<Lms_AccountTypePoco>
    {
        public Lms_AccountTypeLogic(IDataRepository<Lms_AccountTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AccountTypePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_AccountTypePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_AccountTypePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_AccountTypePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_AccountTypePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_AccountTypePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
