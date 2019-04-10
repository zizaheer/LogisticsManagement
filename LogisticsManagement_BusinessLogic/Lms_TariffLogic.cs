using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_TariffLogic : BaseLogic<Lms_TariffPoco>
    {
        public Lms_TariffLogic(IDataRepository<Lms_TariffPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_TariffPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_TariffPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_TariffPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods


        public override Lms_TariffPoco Add(Lms_TariffPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_TariffPoco Update(Lms_TariffPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_TariffPoco poco)
        {
            base.Remove(poco);
        }


        public override void Add(Lms_TariffPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_TariffPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_TariffPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
