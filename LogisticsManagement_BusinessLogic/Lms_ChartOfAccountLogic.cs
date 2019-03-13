using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_ChartOfAccountLogic : BaseLogic<Lms_ChartOfAccountPoco>
    {
        public Lms_ChartOfAccountLogic(IDataRepository<Lms_ChartOfAccountPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_ChartOfAccountPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_ChartOfAccountPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_ChartOfAccountPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_ChartOfAccountPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_ChartOfAccountPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_ChartOfAccountPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
