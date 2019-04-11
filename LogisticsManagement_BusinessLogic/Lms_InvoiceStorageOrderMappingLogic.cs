using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_InvoiceStorageOrderMappingLogic : BaseLogic<Lms_InvoiceStorageOrderMappingPoco>
    {
        IMemoryCache _cache;
        public Lms_InvoiceStorageOrderMappingLogic(IMemoryCache cash, IDataRepository<Lms_InvoiceStorageOrderMappingPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_InvoiceStorageOrderMappingPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_InvoiceStorageOrderMappingPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_InvoiceStorageOrderMappingPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_InvoiceStorageOrderMappingPoco Add(Lms_InvoiceStorageOrderMappingPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_InvoiceStorageOrderMappingPoco Update(Lms_InvoiceStorageOrderMappingPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_InvoiceStorageOrderMappingPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_InvoiceStorageOrderMappingPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_InvoiceStorageOrderMappingPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_InvoiceStorageOrderMappingPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
