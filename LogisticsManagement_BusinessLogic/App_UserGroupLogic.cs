using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_UserGroupLogic : BaseLogic<App_UserGroupPoco>
    {
        public App_UserGroupLogic(IDataRepository<App_UserGroupPoco> repository) : base (repository)
        {
        }

        #region Get Methods

        public override List<App_UserGroupPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_UserGroupPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_UserGroupPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_UserGroupPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_UserGroupPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_UserGroupPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion

        
    }
}
