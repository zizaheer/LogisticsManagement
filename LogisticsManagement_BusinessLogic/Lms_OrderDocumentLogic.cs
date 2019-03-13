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

        public override List<Lms_OrderDocumentPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_OrderDocumentPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_OrderDocumentPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_OrderDocumentPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_OrderDocumentPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_OrderDocumentPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
