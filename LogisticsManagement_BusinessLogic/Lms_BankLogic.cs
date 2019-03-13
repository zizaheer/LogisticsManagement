using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_BankLogic : BaseLogic<Lms_BankPoco>
    {
        public Lms_BankLogic(IDataRepository<Lms_BankPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_BankPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_BankPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_BankPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_BankPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_BankPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_BankPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
