using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_DeliveryOptionLogic : BaseLogic<Lms_DeliveryOptionPoco>
    {
        public Lms_DeliveryOptionLogic(IDataRepository<Lms_DeliveryOptionPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_DeliveryOptionPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_DeliveryOptionPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_DeliveryOptionPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_DeliveryOptionPoco Add(Lms_DeliveryOptionPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_DeliveryOptionPoco Update(Lms_DeliveryOptionPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_DeliveryOptionPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_DeliveryOptionPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_DeliveryOptionPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_DeliveryOptionPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
