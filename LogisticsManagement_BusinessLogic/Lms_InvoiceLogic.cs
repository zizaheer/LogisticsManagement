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

        public override List<Lms_InvoicePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_InvoicePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_InvoicePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_InvoicePoco Add(Lms_InvoicePoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_InvoicePoco Update(Lms_InvoicePoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_InvoicePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_InvoicePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_InvoicePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_InvoicePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
