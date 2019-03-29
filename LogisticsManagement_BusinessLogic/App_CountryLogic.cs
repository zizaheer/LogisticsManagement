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

        public override List<App_CountryPoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_CountryPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_CountryPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override App_CountryPoco Add(App_CountryPoco poco)
        {
            return base.Add(poco);
        }

        public override App_CountryPoco Update(App_CountryPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(App_CountryPoco poco)
        {
            base.Remove(poco);
        }


        public override void Add(App_CountryPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_CountryPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_CountryPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion
    }
}
