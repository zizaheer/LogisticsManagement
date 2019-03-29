using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_ProvinceLogic : BaseLogic<App_ProvincePoco>
    {
        public App_ProvinceLogic(IDataRepository<App_ProvincePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_ProvincePoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_ProvincePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_ProvincePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods


        public override App_ProvincePoco Add(App_ProvincePoco poco)
        {
            return base.Add(poco);
        }

        public override App_ProvincePoco Update(App_ProvincePoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(App_ProvincePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_ProvincePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_ProvincePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_ProvincePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
