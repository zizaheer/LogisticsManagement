using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_PaymentMethodLogic : BaseLogic<Lms_PaymentMethodPoco>
    {
        public Lms_PaymentMethodLogic(IDataRepository<Lms_PaymentMethodPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_PaymentMethodPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_PaymentMethodPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_PaymentMethodPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_PaymentMethodPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_PaymentMethodPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_PaymentMethodPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
