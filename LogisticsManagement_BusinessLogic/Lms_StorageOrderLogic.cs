using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_StorageOrderLogic : BaseLogic<Lms_StorageOrderPoco>
    {
        public Lms_StorageOrderLogic(IDataRepository<Lms_StorageOrderPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_StorageOrderPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_StorageOrderPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_StorageOrderPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_StorageOrderPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_StorageOrderPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_StorageOrderPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
