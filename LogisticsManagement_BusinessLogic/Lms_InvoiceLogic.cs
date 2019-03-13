using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_InvoiceLogic : BaseLogic<Lms_InvoicePoco>
    {
        public Lms_InvoiceLogic(IDataRepository<Lms_InvoicePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_InvoicePoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_InvoicePoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_InvoicePoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_InvoicePoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_InvoicePoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_InvoicePoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
