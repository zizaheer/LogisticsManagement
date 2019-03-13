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

        public override List<Lms_InvoiceWayBillMappingPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_InvoiceWayBillMappingPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_InvoiceWayBillMappingPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_InvoiceWayBillMappingPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_InvoiceWayBillMappingPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_InvoiceWayBillMappingPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
