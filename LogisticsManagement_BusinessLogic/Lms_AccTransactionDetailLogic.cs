using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AccTransactionDetailLogic : BaseLogic<Lms_AccTransactionDetailPoco>
    {
        public Lms_AccTransactionDetailLogic(IDataRepository<Lms_AccTransactionDetailPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AccTransactionDetailPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_AccTransactionDetailPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_AccTransactionDetailPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_AccTransactionDetailPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_AccTransactionDetailPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_AccTransactionDetailPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
