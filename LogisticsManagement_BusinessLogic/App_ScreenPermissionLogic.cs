using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_ScreenPermissionLogic : BaseLogic<App_ScreenPermissionPoco>
    {
        public App_ScreenPermissionLogic(IDataRepository<App_ScreenPermissionPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_ScreenPermissionPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_ScreenPermissionPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_ScreenPermissionPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_ScreenPermissionPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_ScreenPermissionPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_ScreenPermissionPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
