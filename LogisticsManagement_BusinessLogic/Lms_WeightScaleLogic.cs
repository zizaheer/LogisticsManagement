using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_WeightScaleLogic : BaseLogic<Lms_WeightScalePoco>
    {
        public Lms_WeightScaleLogic(IDataRepository<Lms_WeightScalePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_WeightScalePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_WeightScalePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_WeightScalePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_WeightScalePoco Add(Lms_WeightScalePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_WeightScalePoco Update(Lms_WeightScalePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_WeightScalePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_WeightScalePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_WeightScalePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_WeightScalePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion
    }
}
