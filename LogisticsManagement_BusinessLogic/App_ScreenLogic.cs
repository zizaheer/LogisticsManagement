using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_ScreenLogic : BaseLogic<App_ScreenPoco>
    {
        public App_ScreenLogic(IDataRepository<App_ScreenPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_ScreenPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_ScreenPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_ScreenPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_ScreenPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_ScreenPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_ScreenPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
