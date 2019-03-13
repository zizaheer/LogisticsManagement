using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_LoginHistoryLogic : BaseLogic<App_LoginHistoryPoco>
    {
        public App_LoginHistoryLogic(IDataRepository<App_LoginHistoryPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_LoginHistoryPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_LoginHistoryPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_LoginHistoryPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_LoginHistoryPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_LoginHistoryPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_LoginHistoryPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
