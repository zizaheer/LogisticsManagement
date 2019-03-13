using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AccountBalanceLogic : BaseLogic<Lms_AccountBalancePoco>
    {
        public Lms_AccountBalanceLogic(IDataRepository<Lms_AccountBalancePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AccountBalancePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_AccountBalancePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_AccountBalancePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_AccountBalancePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_AccountBalancePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_AccountBalancePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
