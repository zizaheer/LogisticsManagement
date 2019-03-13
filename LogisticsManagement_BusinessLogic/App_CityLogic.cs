using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_CityLogic : BaseLogic<App_CityPoco>
    {
        public App_CityLogic(IDataRepository<App_CityPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_CityPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_CityPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_CityPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_CityPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_CityPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_CityPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
