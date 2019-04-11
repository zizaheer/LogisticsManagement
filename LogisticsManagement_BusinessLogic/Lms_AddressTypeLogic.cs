using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_AddressTypeLogic : BaseLogic<Lms_AddressTypePoco>
    {
        IMemoryCache _cache;
        public Lms_AddressTypeLogic(IMemoryCache cash, IDataRepository<Lms_AddressTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_AddressTypePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_AddressTypePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_AddressTypePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_AddressTypePoco Add(Lms_AddressTypePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_AddressTypePoco Update(Lms_AddressTypePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_AddressTypePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_AddressTypePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_AddressTypePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_AddressTypePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
