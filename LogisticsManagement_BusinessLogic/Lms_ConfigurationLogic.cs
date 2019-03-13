using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_ConfigurationLogic : BaseLogic<Lms_ConfigurationPoco>
    {
        public Lms_ConfigurationLogic(IDataRepository<Lms_ConfigurationPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_ConfigurationPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_ConfigurationPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_ConfigurationPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_ConfigurationPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_ConfigurationPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_ConfigurationPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
