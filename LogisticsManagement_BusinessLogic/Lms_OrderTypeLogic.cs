using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_OrderTypeLogic : BaseLogic<Lms_OrderTypePoco>
    {
        public Lms_OrderTypeLogic(IDataRepository<Lms_OrderTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_OrderTypePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_OrderTypePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_OrderTypePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_OrderTypePoco Add(Lms_OrderTypePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_OrderTypePoco Update(Lms_OrderTypePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_OrderTypePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_OrderTypePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_OrderTypePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_OrderTypePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
