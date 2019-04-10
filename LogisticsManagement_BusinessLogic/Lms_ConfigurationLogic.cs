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

        public override List<Lms_ConfigurationPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_ConfigurationPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_ConfigurationPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_ConfigurationPoco Add(Lms_ConfigurationPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_ConfigurationPoco Update(Lms_ConfigurationPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_ConfigurationPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_ConfigurationPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_ConfigurationPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_ConfigurationPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
