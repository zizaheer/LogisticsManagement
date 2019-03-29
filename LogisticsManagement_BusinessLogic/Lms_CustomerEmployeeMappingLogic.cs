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

        public override List<Lms_CustomerEmployeeMappingPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_CustomerEmployeeMappingPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_CustomerEmployeeMappingPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_CustomerEmployeeMappingPoco Add(Lms_CustomerEmployeeMappingPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_CustomerEmployeeMappingPoco Update(Lms_CustomerEmployeeMappingPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_CustomerEmployeeMappingPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_CustomerEmployeeMappingPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_CustomerEmployeeMappingPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_CustomerEmployeeMappingPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
