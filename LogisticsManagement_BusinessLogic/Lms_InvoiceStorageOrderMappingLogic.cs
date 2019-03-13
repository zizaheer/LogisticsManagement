using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_InvoiceStorageOrderMappingLogic : BaseLogic<Lms_InvoiceStorageOrderMappingPoco>
    {
        public Lms_InvoiceStorageOrderMappingLogic(IDataRepository<Lms_InvoiceStorageOrderMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_InvoiceStorageOrderMappingPoco> GetAllList()
        {
            return base.GetAllList();
        }

        public override List<Lms_InvoiceStorageOrderMappingPoco> GetFilteredList(int id)
        {
            return base.GetFilteredList(id);
        }

        public override Lms_InvoiceStorageOrderMappingPoco GetSinglePoco(int id)
        {
            return base.GetSinglePoco(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override void Add(Lms_InvoiceStorageOrderMappingPoco[] userGroupPocos)
        {
            base.Add(userGroupPocos);
        }

        public override void Update(Lms_InvoiceStorageOrderMappingPoco[] userGroupPocos)
        {
            base.Update(userGroupPocos);
        }

        public override void Remove(Lms_InvoiceStorageOrderMappingPoco[] userGroupPocos)
        {
            base.Remove(userGroupPocos);
        }

        #endregion


    }
}
