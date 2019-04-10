using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_OrderDocumentLogic : BaseLogic<Lms_OrderDocumentPoco>
    {
        public Lms_OrderDocumentLogic(IDataRepository<Lms_OrderDocumentPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_OrderDocumentPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_OrderDocumentPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_OrderDocumentPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_OrderDocumentPoco Add(Lms_OrderDocumentPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_OrderDocumentPoco Update(Lms_OrderDocumentPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_OrderDocumentPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_OrderDocumentPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_OrderDocumentPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_OrderDocumentPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
