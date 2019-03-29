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

        public override List<App_CityPoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_CityPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_CityPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override App_CityPoco Add(App_CityPoco poco)
        {
            return base.Add(poco);
        }

        public override App_CityPoco Update(App_CityPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(App_CityPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_CityPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_CityPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_CityPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
