using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_TransactionLogic : BaseLogic<Lms_TransactionPoco>
    {
        public Lms_TransactionLogic(IDataRepository<Lms_TransactionPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_TransactionPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_TransactionPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_TransactionPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_TransactionPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_TransactionPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_TransactionPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
