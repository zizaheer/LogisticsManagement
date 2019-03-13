using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AddressLogic : BaseLogic<Lms_AddressPoco>
    {
        public Lms_AddressLogic(IDataRepository<Lms_AddressPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AddressPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_AddressPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_AddressPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_AddressPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_AddressPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_AddressPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
