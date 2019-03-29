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

        public override List<Lms_AccountBalancePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_AccountBalancePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_AccountBalancePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AccountBalancePoco Add(Lms_AccountBalancePoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_AccountBalancePoco Update(Lms_AccountBalancePoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_AccountBalancePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_AccountBalancePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_AccountBalancePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_AccountBalancePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
