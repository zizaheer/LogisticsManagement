using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_UserLogic : BaseLogic<App_UserPoco>
    {
        public App_UserLogic(IDataRepository<App_UserPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_UserPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_UserPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_UserPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_UserPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_UserPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_UserPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
