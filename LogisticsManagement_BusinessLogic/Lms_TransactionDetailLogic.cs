using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_TransactionDetailLogic : BaseLogic<Lms_TransactionDetailPoco>
    {
        IMemoryCache _cache;
        public Lms_TransactionDetailLogic(IMemoryCache cash, IDataRepository<Lms_TransactionDetailPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_TransactionDetailPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_TransactionDetailPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_TransactionDetailPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_TransactionDetailPoco Add(Lms_TransactionDetailPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_TransactionDetailPoco Update(Lms_TransactionDetailPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_TransactionDetailPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_TransactionDetailPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_TransactionDetailPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_TransactionDetailPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion

    }
}
