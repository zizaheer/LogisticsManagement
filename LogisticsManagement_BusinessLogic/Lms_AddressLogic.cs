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

        public override List<Lms_AddressPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_AddressPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_AddressPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AddressPoco Add(Lms_AddressPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_AddressPoco Update(Lms_AddressPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_AddressPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_AddressPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_AddressPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_AddressPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
