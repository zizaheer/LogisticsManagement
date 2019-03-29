using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AddressMappingLogic : BaseLogic<Lms_AddressMappingPoco>
    {
        public Lms_AddressMappingLogic(IDataRepository<Lms_AddressMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AddressMappingPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_AddressMappingPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_AddressMappingPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AddressMappingPoco Add(Lms_AddressMappingPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_AddressMappingPoco Update(Lms_AddressMappingPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_AddressMappingPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_AddressMappingPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_AddressMappingPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_AddressMappingPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
