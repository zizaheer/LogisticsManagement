using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_StorageOrderDeliveryLogic : BaseLogic<Lms_StorageOrderDeliveryPoco>
    {
        public Lms_StorageOrderDeliveryLogic(IDataRepository<Lms_StorageOrderDeliveryPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_StorageOrderDeliveryPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_StorageOrderDeliveryPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_StorageOrderDeliveryPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_StorageOrderDeliveryPoco Add(Lms_StorageOrderDeliveryPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_StorageOrderDeliveryPoco Update(Lms_StorageOrderDeliveryPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_StorageOrderDeliveryPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_StorageOrderDeliveryPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_StorageOrderDeliveryPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_StorageOrderDeliveryPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
