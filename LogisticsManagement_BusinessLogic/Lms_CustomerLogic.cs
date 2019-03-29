using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CustomerLogic : BaseLogic<Lms_CustomerPoco>
    {
        public Lms_CustomerLogic(IDataRepository<Lms_CustomerPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_CustomerPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_CustomerPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_CustomerPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_CustomerPoco Add(Lms_CustomerPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_CustomerPoco Update(Lms_CustomerPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_CustomerPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_CustomerPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_CustomerPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_CustomerPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
