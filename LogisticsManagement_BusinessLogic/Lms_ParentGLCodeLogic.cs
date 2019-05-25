using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_ParentGLCodeLogic : BaseLogic<Lms_ParentGLCodePoco>
    {

        IMemoryCache _cache;
        public Lms_ParentGLCodeLogic(IMemoryCache cache, IDataRepository<Lms_ParentGLCodePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_ParentGLCodePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_ParentGLCodePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_ParentGLCodePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_ParentGLCodePoco Add(Lms_ParentGLCodePoco poco)
        {
            poco.CreateDate = DateTime.Now;
            var addedPoco = base.Add(poco);

            return addedPoco;
        }

        public override Lms_ParentGLCodePoco Update(Lms_ParentGLCodePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);

            return updatedPoco;
        }

        public override void Remove(Lms_ParentGLCodePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_ParentGLCodePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_ParentGLCodePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_ParentGLCodePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}

