using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_ProvinceLogic : BaseLogic<App_ProvincePoco>
    {
        public App_ProvinceLogic(IDataRepository<App_ProvincePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_ProvincePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_ProvincePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_ProvincePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_ProvincePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_ProvincePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_ProvincePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
