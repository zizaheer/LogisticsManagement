using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_CountryLogic : BaseLogic<App_CountryPoco>
    {
        public App_CountryLogic(IDataRepository<App_CountryPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_CountryPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<App_CountryPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override App_CountryPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(App_CountryPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(App_CountryPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(App_CountryPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
