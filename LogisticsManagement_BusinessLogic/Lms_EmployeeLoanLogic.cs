using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeLoanLogic : BaseLogic<Lms_EmployeeLoanPoco>
    {
        IMemoryCache _cache;
        public Lms_EmployeeLoanLogic(IMemoryCache cash, IDataRepository<Lms_EmployeeLoanPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeeLoanPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeeLoanPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeeLoanPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeeLoanPoco Add(Lms_EmployeeLoanPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_EmployeeLoanPoco Update(Lms_EmployeeLoanPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeeLoanPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeeLoanPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeeLoanPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeeLoanPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
