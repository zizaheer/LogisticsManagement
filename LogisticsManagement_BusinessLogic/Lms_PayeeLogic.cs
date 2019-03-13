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

        public override List<Lms_PayeePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_PayeePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_PayeePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_PayeePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_PayeePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_PayeePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
