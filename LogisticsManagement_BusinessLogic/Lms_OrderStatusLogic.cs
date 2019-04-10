using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_OrderStatusLogic : BaseLogic<Lms_OrderStatusPoco>
    {
        public Lms_OrderStatusLogic(IDataRepository<Lms_OrderStatusPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_OrderStatusPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_OrderStatusPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_OrderStatusPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_OrderStatusPoco Add(Lms_OrderStatusPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_OrderStatusPoco Update(Lms_OrderStatusPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_OrderStatusPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_OrderStatusPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_OrderStatusPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_OrderStatusPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
