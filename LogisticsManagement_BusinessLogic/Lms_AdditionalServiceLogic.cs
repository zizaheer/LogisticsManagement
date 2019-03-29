using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AdditionalServiceLogic : BaseLogic<Lms_AdditionalServicePoco>
    {
        public Lms_AdditionalServiceLogic(IDataRepository<Lms_AdditionalServicePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AdditionalServicePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_AdditionalServicePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_AdditionalServicePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AdditionalServicePoco Add(Lms_AdditionalServicePoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_AdditionalServicePoco Update(Lms_AdditionalServicePoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_AdditionalServicePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_AdditionalServicePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_AdditionalServicePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_AdditionalServicePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
