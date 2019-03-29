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

        public override List<App_ScreenPoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_ScreenPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_ScreenPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods


        public override App_ScreenPoco Add(App_ScreenPoco poco)
        {
            return base.Add(poco);
        }

        public override App_ScreenPoco Update(App_ScreenPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(App_ScreenPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_ScreenPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_ScreenPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_ScreenPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
