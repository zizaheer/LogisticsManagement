using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class App_LoginHistoryLogic : BaseLogic<App_LoginHistoryPoco>
    {
        IMemoryCache _cache;
        public App_LoginHistoryLogic(IMemoryCache cash, IDataRepository<App_LoginHistoryPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_LoginHistoryPoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_LoginHistoryPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_LoginHistoryPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods


        public override App_LoginHistoryPoco Add(App_LoginHistoryPoco poco)
        {
            return base.Add(poco);
        }

        public override App_LoginHistoryPoco Update(App_LoginHistoryPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(App_LoginHistoryPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_LoginHistoryPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_LoginHistoryPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_LoginHistoryPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
