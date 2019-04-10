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

        public override List<App_UserGroupPoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_UserGroupPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_UserGroupPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override App_UserGroupPoco Add(App_UserGroupPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override App_UserGroupPoco Update(App_UserGroupPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(App_UserGroupPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_UserGroupPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_UserGroupPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_UserGroupPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion

        
    }
}
