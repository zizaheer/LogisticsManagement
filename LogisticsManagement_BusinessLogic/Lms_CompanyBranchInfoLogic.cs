using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CompanyBranchInfoLogic : BaseLogic<Lms_CompanyBranchInfoPoco>
    {
        public Lms_CompanyBranchInfoLogic(IDataRepository<Lms_CompanyBranchInfoPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_CompanyBranchInfoPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_CompanyBranchInfoPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_CompanyBranchInfoPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_CompanyBranchInfoPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_CompanyBranchInfoPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_CompanyBranchInfoPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
