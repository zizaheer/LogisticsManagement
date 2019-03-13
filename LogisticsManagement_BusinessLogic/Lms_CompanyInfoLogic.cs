using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CompanyInfoLogic : BaseLogic<Lms_CompanyInfoPoco>
    {
        public Lms_CompanyInfoLogic(IDataRepository<Lms_CompanyInfoPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_CompanyInfoPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_CompanyInfoPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_CompanyInfoPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_CompanyInfoPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_CompanyInfoPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_CompanyInfoPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
