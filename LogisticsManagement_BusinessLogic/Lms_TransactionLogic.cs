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

        public override List<Lms_TransactionPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_TransactionPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_TransactionPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_TransactionPoco Add(Lms_TransactionPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_TransactionPoco Update(Lms_TransactionPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_TransactionPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_TransactionPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_TransactionPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_TransactionPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
