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

        public override List<Lms_AccTransactionDetailPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_AccTransactionDetailPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_AccTransactionDetailPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AccTransactionDetailPoco Add(Lms_AccTransactionDetailPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_AccTransactionDetailPoco Update(Lms_AccTransactionDetailPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_AccTransactionDetailPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_AccTransactionDetailPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_AccTransactionDetailPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_AccTransactionDetailPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
