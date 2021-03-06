﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class App_DocumentCategoryLogic : BaseLogic<App_DocumentCategoryPoco>
    {
        IMemoryCache _cache;
        public App_DocumentCategoryLogic(IMemoryCache cash, IDataRepository<App_DocumentCategoryPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_DocumentCategoryPoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_DocumentCategoryPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_DocumentCategoryPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override App_DocumentCategoryPoco Add(App_DocumentCategoryPoco poco)
        {
            poco.CreateDate = DateTime.Now;
            return base.Add(poco);
        }

        public override App_DocumentCategoryPoco Update(App_DocumentCategoryPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(App_DocumentCategoryPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_DocumentCategoryPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_DocumentCategoryPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_DocumentCategoryPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
