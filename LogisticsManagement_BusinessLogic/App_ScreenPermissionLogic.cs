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

        public override List<App_ScreenPermissionPoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_ScreenPermissionPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_ScreenPermissionPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override App_ScreenPermissionPoco Add(App_ScreenPermissionPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override App_ScreenPermissionPoco Update(App_ScreenPermissionPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(App_ScreenPermissionPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_ScreenPermissionPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_ScreenPermissionPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_ScreenPermissionPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
