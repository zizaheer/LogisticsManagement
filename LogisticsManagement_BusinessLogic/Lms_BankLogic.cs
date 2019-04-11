using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_BankLogic : BaseLogic<Lms_BankPoco>
    {
        IMemoryCache _cache;
        public Lms_BankLogic(IMemoryCache cash, IDataRepository<Lms_BankPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_BankPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_BankPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_BankPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_BankPoco Add(Lms_BankPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_BankPoco Update(Lms_BankPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_BankPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_BankPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_BankPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_BankPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
