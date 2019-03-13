using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_DocumentTypeLogic : BaseLogic<App_DocumentTypePoco>
    {
        public App_DocumentTypeLogic(IDataRepository<App_DocumentTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_DocumentTypePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_DocumentTypePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_DocumentTypePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_DocumentTypePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_DocumentTypePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_DocumentTypePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
