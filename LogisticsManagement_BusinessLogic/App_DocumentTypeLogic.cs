﻿using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class App_DocumentTypeLogic : BaseLogic<App_DocumentTypePoco>
    {
        public App_DocumentTypeLogic(IDataRepository<App_DocumentTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_DocumentTypePoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_DocumentTypePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_DocumentTypePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods


        public override App_DocumentTypePoco Add(App_DocumentTypePoco poco)
        {
            return base.Add(poco);
        }

        public override App_DocumentTypePoco Update(App_DocumentTypePoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(App_DocumentTypePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_DocumentTypePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(App_DocumentTypePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(App_DocumentTypePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
