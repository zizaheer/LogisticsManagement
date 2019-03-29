using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_InvoiceWayBillMappingLogic : BaseLogic<Lms_InvoiceWayBillMappingPoco>
    {
        public Lms_InvoiceWayBillMappingLogic(IDataRepository<Lms_InvoiceWayBillMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_InvoiceWayBillMappingPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_InvoiceWayBillMappingPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_InvoiceWayBillMappingPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_InvoiceWayBillMappingPoco Add(Lms_InvoiceWayBillMappingPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_InvoiceWayBillMappingPoco Update(Lms_InvoiceWayBillMappingPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_InvoiceWayBillMappingPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_InvoiceWayBillMappingPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_InvoiceWayBillMappingPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_InvoiceWayBillMappingPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
