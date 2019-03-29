using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_PayeeLogic : BaseLogic<Lms_PayeePoco>
    {
        public Lms_PayeeLogic(IDataRepository<Lms_PayeePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_PayeePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_PayeePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_PayeePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_PayeePoco Add(Lms_PayeePoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_PayeePoco Update(Lms_PayeePoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_PayeePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_PayeePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_PayeePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_PayeePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
