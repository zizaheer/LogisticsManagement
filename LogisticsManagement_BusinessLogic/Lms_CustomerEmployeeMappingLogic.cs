using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CustomerEmployeeMappingLogic : BaseLogic<Lms_CustomerEmployeeMappingPoco>
    {
        public Lms_CustomerEmployeeMappingLogic(IDataRepository<Lms_CustomerEmployeeMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_CustomerEmployeeMappingPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_CustomerEmployeeMappingPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_CustomerEmployeeMappingPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_CustomerEmployeeMappingPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_CustomerEmployeeMappingPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_CustomerEmployeeMappingPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
