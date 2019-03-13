using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_DocumentCategoryLogic : BaseLogic<App_DocumentCategoryPoco>
    {
        public App_DocumentCategoryLogic(IDataRepository<App_DocumentCategoryPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_DocumentCategoryPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_DocumentCategoryPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_DocumentCategoryPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_DocumentCategoryPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_DocumentCategoryPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_DocumentCategoryPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
