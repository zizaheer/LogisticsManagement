using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_TariffLogic : BaseLogic<Lms_TariffPoco>
    {
        public Lms_TariffLogic(IDataRepository<Lms_TariffPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_TariffPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_TariffPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_TariffPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods


        //public override void AddSingle(Lms_TariffPoco userGroupPoco)
        //{
        //    base.AddSingle(userGroupPoco);
        //}

        //public override void UpdateSingle(Lms_TariffPoco userGroupPoco)
        //{
        //    base.UpdateSingle(userGroupPoco);
        //}

        //public override void RemoveSingle(Lms_TariffPoco userGroupPoco)
        //{
        //    base.RemoveSingle(userGroupPoco);
        //}


        public override void Add(Lms_TariffPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_TariffPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_TariffPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
